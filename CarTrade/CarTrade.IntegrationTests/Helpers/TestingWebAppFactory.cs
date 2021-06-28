﻿using CarTrade.Data;
using CarTrade.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarTrade.IntegrationTests.Helpers
{
    public class TestingWebAppFactory<T> : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                  d => d.ServiceType ==
                     typeof(DbContextOptions<CarDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                var serviceProvider = new ServiceCollection()
                  .AddEntityFrameworkInMemoryDatabase()
                  .BuildServiceProvider();
                services.AddDbContext<CarDbContext>(options =>
                {
                    options.UseInMemoryDatabase("Car");
                    options.UseInternalServiceProvider(serviceProvider);
                });
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    //using (var appContext = scope.ServiceProvider.GetRequiredService<EmployeeContext>())
                    //{
                    //    try
                    //    {
                    //        appContext.Database.EnsureCreated();
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        //Log errors or do anything you think it's needed
                    //        throw;
                    //    }
                    //}
                }
            });
        }
    }
}
