using AutoMapper;
using CarTrade.Data;
using CarTrade.Data.Models;
using CarTrade.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hangfire;
using Hangfire.SqlServer;
using System;
using CarTrade.Services.InsurancePolicies;
using CarTrade.Services.Vignettes;

using static CarTrade.Web.WebConstants;
using Hangfire.Dashboard;

namespace CarTrade.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CarDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            this.ConfigureHangfire(services);

            services.AddHangfireServer();

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                //options.SignIn.RequireConfirmedAccount = true;
            })
                .AddDefaultUI()
                .AddEntityFrameworkStores<CarDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                    {
                        facebookOptions.AppId = Configuration["Authentication:Facebook:Id"];
                        facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                    }
                );

            services.AddDomainServices();

            services.AddAutoMapper();

            //friendly url
            //services.AddRouting(routing => routing.LowercaseUrls = true);

            //services.AddControllersWithViews(options =>
            //    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager recurringJobManager)
        {
            this.SeedHangfireJobs(recurringJobManager);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseDatabaseMigration();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseHangfireDashboard();
            //backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
            //RecurringJob.AddOrUpdate(() => Console.WriteLine("RecurringJob!"), Cron.Minutely);

            app.UseHangfireServer(new BackgroundJobServerOptions { WorkerCount = 2 });
            app.UseHangfireDashboard(
                "/Administration/HangFire",
                new DashboardOptions { Authorization = new[] { new HangfireAuthorizationFilter() } });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        protected virtual void ConfigureHangfire(IServiceCollection services)
        {
            services.AddHangfire(configuration => configuration
                   .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                   .UseSimpleAssemblyNameTypeSerializer()
                   .UseRecommendedSerializerSettings()
                   .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                   {
                       CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                       SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                       QueuePollInterval = TimeSpan.Zero,
                       UseRecommendedIsolationLevel = true,
                       DisableGlobalLocks = true
                   }));
        }

        private void SeedHangfireJobs(IRecurringJobManager recurringJobManager)
        {
            recurringJobManager.AddOrUpdate<InsurancesPoliciesService>("SetExpiredInsurancePoliciesLogic",
                x => x.SetExpiredInsurancePoliciesLogicAsync(), Cron.Daily);
            recurringJobManager.AddOrUpdate<VignettesService>("SetExpiredVignetteLogic",
               x => x.SetVignetteExpireLogicAsync(), Cron.Daily);            
        }

        public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                var httpContext = context.GetHttpContext();
                return httpContext.User.IsInRole(AdministratorRole);
            }
        }
    }
}
