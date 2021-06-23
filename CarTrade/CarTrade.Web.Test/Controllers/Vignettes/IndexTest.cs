using AutoMapper;
using CarTrade.Data.Models;
using CarTrade.Services.Tests.Enums;
using CarTrade.Services.Vehicles;
using CarTrade.Services.Vignettes;
using CarTrade.Services.Vignettes.Models;
using CarTrade.Web.Controllers;
using CarTrade.Web.Models.Vignettes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CarTrade.Web.Test.Controllers.Vignettes
{
    public class IndexTest
    {
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfVignettes()
        {
            // Arrange
            var mockVehicleService = new Mock<IVehicleService>();
            var mockVignetteService = new Mock<IVignettesService>();
            var mockImapper = new Mock<IMapper>();

            mockVignetteService.Setup(service => service.AllAsync())
                .ReturnsAsync(AllVignettes());

            var controller = new VignettesController(mockVignetteService.Object,
                mockVehicleService.Object, mockImapper.Object);

            //Act
            var result = await controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<VignetteListingViewModel>(
                viewResult.ViewData.Model);
            Assert.Equal(18, model.AllVignettes.Count());
        }
              
        private VignetteListingViewModel GetVignetteListingViewModel()
            => new VignetteListingViewModel
                {
                    AllVignettes = AllVignettes()
                };

        private IEnumerable<VignetteListingServiceModel> AllVignettes()
            => new List<VignetteListingServiceModel>()
            {
                 //Old time Expired Vignette with asign
                new VignetteListingServiceModel
                {
                    Id = 18,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays) + (int)TimesPeriod.YearDays),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays),
                    Expired = true,
                    VehicleId = 1
                },
                //Expired Vignette with asign
                new VignetteListingServiceModel
                {
                    Id = 1,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                    Expired = true,
                    VehicleId = 1
                },
                //Active Vignette
                new VignetteListingServiceModel
                {
                    Id = 2,
                    StartDate =  DateTime.UtcNow.AddDays(-((int)TimesPeriod.MonthlyDays * 2 + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.MonthlyDays * 2)),
                    Expired = false,
                    VehicleId = 1
                },
                //Expired Vignette with asign
                 new VignetteListingServiceModel
                 {
                    Id = 3,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                    Expired = true,
                    VehicleId = 2
                },
                 //Active Vignette
                new VignetteListingServiceModel
                {
                    Id = 4,
                    StartDate =  DateTime.UtcNow.AddDays(-((int)TimesPeriod.MonthlyDays * 3 + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.MonthlyDays * 3)),
                    Expired = false,
                    VehicleId = 2
                },
                //Active Vignette
                new VignetteListingServiceModel {
                    Id = 5,
                    StartDate =  DateTime.UtcNow.AddDays(-((int)TimesPeriod.MonthlyDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.MonthlyDays)),
                    Expired = false,
                    VehicleId = 3
                },
                //Active Vignette
                new VignetteListingServiceModel
                {
                    Id = 6,
                    StartDate =  DateTime.UtcNow.AddDays(-((int)TimesPeriod.WeeklyDays * 3)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.WeeklyDays * 3 + (int)TimesPeriod.YearDays)),
                    Expired = false,
                    VehicleId = 4
                },
                //Active Start today Vignette
                new VignetteListingServiceModel
                {
                    Id = 7,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays((int)TimesPeriod.YearDays),
                    Expired = false,
                    VehicleId = 5
                },
                //Active Start Vignette
                new VignetteListingServiceModel
                {
                    Id = 8,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays((int)TimesPeriod.YearDays),
                    Expired = false,
                    VehicleId = 6
                },
                //Expiring Today Vignette
                new VignetteListingServiceModel
                {
                    Id = 9,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow,
                    Expired = false,
                    VehicleId = 7
                },
                //Expiring Today Vignette
                new VignetteListingServiceModel
                {
                    Id = 10,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow,
                    Expired = false,
                    VehicleId = 8
                },
                //Expire but not asign
                new VignetteListingServiceModel
                {
                    Id = 11,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                    Expired = false,
                    VehicleId = 9
                },
                //Expired with asign
                new VignetteListingServiceModel
                {
                    Id = 12,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                    Expired = true,
                    VehicleId = 10
                },
                 //Expired but not asign
                new VignetteListingServiceModel
                {
                    Id = 13,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                    Expired = false,
                    VehicleId = 10
                },                
                 //Expired after 30 days
                new VignetteListingServiceModel
                {
                    Id = 14,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.MonthlyDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays((((int)TimesPeriod.MonthlyDays) - 1)),
                    Expired = false,
                    VehicleId = 12
                },
                //Expired after 20 days
                new VignetteListingServiceModel
                {
                    Id = 15,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.MonthlyDays + 11)),
                    EndDate = DateTime.UtcNow.AddDays((((int)TimesPeriod.MonthlyDays) - 11)),
                    Expired = false,
                    VehicleId = 13
                },
                //Expired after 7 days
                new VignetteListingServiceModel
                {
                    Id = 16,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.WeeklyDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.WeeklyDays)),
                    Expired = false,
                    VehicleId = 14
                },
                //Expired after 1 day
                new VignetteListingServiceModel
                {
                    Id = 17,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.Dayly + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.Dayly)),
                    Expired = false,
                    VehicleId = 15
                }
            };

    }
}
