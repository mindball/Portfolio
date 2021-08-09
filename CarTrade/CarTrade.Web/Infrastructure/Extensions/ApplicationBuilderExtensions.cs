using CarTrade.Data;
using CarTrade.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using CarTrade.Common;

namespace CarTrade.Web.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDatabaseMigration(this IApplicationBuilder app)
        {
            using (var serviceScope =
                app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var defaultDbContext = serviceScope.ServiceProvider.GetService<CarDbContext>();
                //defaultDbContext.Database.EnsureCreated();
                defaultDbContext.Database.Migrate();
                                
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                Task
                   .Run(async () =>
                   {
                       var adminName = DataConstants.AdministratorRole;
                       var roles = new[]
                       {
                            adminName,
                            DataConstants.ManagerRole,
                            DataConstants.DriverRole
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
                           var headOffice = new Branch { Town = "Стара Загора", Address = "бул. \"Никола Петков 55\"" };
                           var employer = new Employer { Name = "Автохит Трейд ООД" };
                           adminUser = new User
                           {
                               Email = adminEmail,
                               UserName = adminName,
                               FirstName = adminName,
                               SecondName = adminName,
                               LastName = adminName,
                               Branch = headOffice,
                               Employer = employer
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
