using CarTrade.Data;
using CarTrade.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace CarTrade.Web.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDatabaseMigration(this IApplicationBuilder app)
        {
            using (var serviceScope =
                app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<CarDbContext>();
                //dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();

                
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                Task
                   .Run(async () =>
                   {
                       var adminName = WebConstants.AdministratorRole;
                       var roles = new[]
                       {
                            adminName,
                            WebConstants.BlogAuthorRole,
                            WebConstants.DriverRole
                        };

                       foreach (var role in roles)
                       {
                           var roleExists = await roleManager.RoleExistsAsync(role);

                           if (!roleExists)
                           {
                               await roleManager.CreateAsync(new IdentityRole
                               {
                                   Name = role
                               });
                           }
                       }

                       var adminEmail = "admin@admin.com";
                       var adminUser = await userManager.FindByEmailAsync(adminEmail);

                       if (adminUser == null)
                       {
                           adminUser = new User
                           {
                               Email = adminEmail,
                               UserName = adminName,
                               FirstName = adminName,
                               SecondName = adminName,
                               LastName = adminName,
                               BranchId = 1
                           };

                           await userManager.CreateAsync(adminUser, "1q2w3e4rAA");
                           await userManager.AddToRoleAsync(adminUser, adminName);
                       }
                   }).Wait();
            }

            return app;
        }
    }
}
