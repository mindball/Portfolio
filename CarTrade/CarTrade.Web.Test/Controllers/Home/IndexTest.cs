using CarTrade.Data.Enums;
using CarTrade.Data.Models;
using CarTrade.Services.Branches;
using CarTrade.Services.Branches.Models;
using CarTrade.Services.InsurancePolicies;
using CarTrade.Services.Tests;
using CarTrade.Services.Tests.Enums;
using CarTrade.Services.Vehicles;
using CarTrade.Services.Vehicles.Models;
using CarTrade.Services.Vignettes;
using CarTrade.Web.Controllers;
using CarTrade.Web.Models.Home;
using FluentAssertions;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CarTrade.Web.Test.Controllers.Home
{
    [Collection("Web collection")]
    public class IndexTest
    {
        private DatabaseFixture fixture;

        public IndexTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task Index_ReturnViewModelOfCollectionOfExpireDataForAllBranches()
        {
            //Arrange  
            //var hangFireClient = GetHangFire();

            var mockBranchService = GetMockBranchService();
            var mockInsuranceService = GetMockInsuranceService();
            var mockVehicleService = GetMockVehicleService();
            var mockVignettesService = GetMockVignettesService();
            var mockRecurring = GetMockRecurring();

            SetupMockObjects(mockBranchService, mockInsuranceService, mockVignettesService);

            var homeController = new HomeController(
                null,
                mockBranchService.Object,
                mockInsuranceService.Object,
                mockVehicleService.Object,
                mockVignettesService.Object,
                mockRecurring.Object);
            //Act
            var result = await homeController.Index();

            //Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<ListExpireDataForAllBranchesViewModel>>(
                    viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Index_ShouldReturnNotFoundWithEmptyBranchCollection()
        {
            //Arrange
            var mockBranchService = GetMockBranchService();
            var mockInsuranceService = GetMockInsuranceService();
            var mockVehicleService = GetMockVehicleService();
            var mockVignettesService = GetMockVignettesService();
            var mockRecurring = GetMockRecurring();

            SetupMockObjects(mockBranchService, mockInsuranceService, mockVignettesService);

            foreach (var item in this.fixture.Context.Branches)
            {
                this.fixture.Context.Branches.Remove(item);
                await this.fixture.Context.SaveChangesAsync();
            }

            var homeController = new HomeController(
                null,
                mockBranchService.Object,
                mockInsuranceService.Object,
                mockVehicleService.Object,
                mockVignettesService.Object,
                mockRecurring.Object);

            //Act
            var actionResult = await homeController.Index();

            //Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        //TODO: How to test null collection List<ListExpireDataForAllBranchesViewModel

        [Theory]
        [InlineData(5, 4, 2, 2)]
        public async Task Index_ShouldReturnCorrectExpireData(
            int expectedVignetteCount, 
            int expectedInspectionCount, 
            int expectedInsuranceCount, 
            int expectedOilDistanceCount)
        {
            //Arrange
            const int branchId = 1;
            var mockInsuranceService = GetMockInsuranceService();
            var mockVehicleService = GetMockVehicleService();
            var mockVignettesService = GetMockVignettesService();
            var mockRecurring = GetMockRecurring();
            var mockBranchService = GetMockBranchService();

            List<BranchListingServiceModel> allBranches = CollectionOfBranches(branchId);
            List<VehicleListExpireInsurancePolicyServiceModel> vehiclesWithExpireInsurance = CollectionOfInsurancePolicy();
            List<VehicleListExpireVignetteServiceModel> vehicleWithExpireVignette = CollectionOfExpiringOrExpiredVignettes();
            List<VehicleListingChangeOilServiceModel> vehicleWithExpireOilDistance = CollectionOfVehiclesWithExpireOilDistance();
            List<VehicleListingInspectionSafetyCheckServiceModel> vehicleWithExpireInspectionCheck = CollcectionOfVehicleWithExpireInspectionCheck();
            
            mockBranchService.Setup(cfg => cfg.AllAsync())
                    .ReturnsAsync(allBranches);
            mockVehicleService.Setup(cfg => cfg.GetInsuranceExpireDataAsync(branchId))
                .ReturnsAsync(vehiclesWithExpireInsurance);
            mockVehicleService.Setup(cfg => cfg.GetVignetteExpireDataAsync(branchId))
                .ReturnsAsync(vehicleWithExpireVignette);
            mockVehicleService.Setup(cfg => cfg.GetOilChangeExpireDataAsync(branchId))
                .ReturnsAsync(vehicleWithExpireOilDistance);
            mockVehicleService.Setup(cfg => cfg.GetInspectionSafetyCheckExpireDataAsync(branchId))
                .ReturnsAsync(vehicleWithExpireInspectionCheck);

            var homeController = new HomeController(
                null,
                mockBranchService.Object,
                mockInsuranceService.Object,
                mockVehicleService.Object,
                mockVignettesService.Object,
                mockRecurring.Object);

            //Act
            var result = await homeController.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model;
            var concretteModel = model as List<ListExpireDataForAllBranchesViewModel>;
            var actualVignetteCount = concretteModel.Select(x => x.VehiclesWithExpireVignettes.Count());
            var actualInsuranceCount = concretteModel.SelectMany(x => x.VehiclesWithExpirePolicy.Select(x => x.InsurancePolicies.Count()));
            var actualOilDistanceCount = concretteModel.Select(x => x.VehiclesWithOilChangeDistance.Count());
            var actualInspectionCount = concretteModel.Select(x => x.VehiclesWithInspectionExpire.Count());

            //Assert
            Assert.IsType<List<ListExpireDataForAllBranchesViewModel>>(model);

            Assert.Equal(expectedVignetteCount, actualVignetteCount.FirstOrDefault());
            Assert.Equal(expectedInsuranceCount, actualInsuranceCount.FirstOrDefault());
            Assert.Equal(expectedOilDistanceCount, actualOilDistanceCount.FirstOrDefault());
            Assert.Equal(expectedInspectionCount, actualInspectionCount.FirstOrDefault());
        }

        private List<VehicleListingInspectionSafetyCheckServiceModel> CollcectionOfVehicleWithExpireInspectionCheck()
        => new List<VehicleListingInspectionSafetyCheckServiceModel>
            {
                //Inspection expire monthly
                new VehicleListingInspectionSafetyCheckServiceModel
                {
                    VehicleId = 1,
                    PlateNumber = "CT1234BM",
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.HalfYearDays),
                    Vin = "VWVZZZ1KZ1P324444"
                },
                //Inspection expire monthly
                new VehicleListingInspectionSafetyCheckServiceModel
                {
                    VehicleId = 3,
                    PlateNumber = "CT7777AM",
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.MonthlyDays - 1),
                    Vin = "VWVZZZ1KZ1P999888"
                },
                 //InspectionSafetyCheck expire today
                new VehicleListingInspectionSafetyCheckServiceModel
                {
                    VehicleId = 4,
                    PlateNumber = "CT9876AM",
                    InspectionSafetyCheck = DateTime.UtcNow,
                    Vin = "VWVZZZ1KZ1P333333"
                },
                 //InspectionSafetyCheck expire today
                new VehicleListingInspectionSafetyCheckServiceModel
                {
                    VehicleId = 5,
                    PlateNumber = "AAAAAA",
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays(-(int)TimesPeriod.Dayly),
                    Vin = "AAAAAAAAA"
                },
            };

        private List<VehicleListExpireVignetteServiceModel> CollectionOfExpiringOrExpiredVignettes()
            => new List<VehicleListExpireVignetteServiceModel>()
            {
                 //Expiring Today Vignette
                new VehicleListExpireVignetteServiceModel
                {
                    ExpireDate =  DateTime.UtcNow,
                    PlateNumber = "CT1111AM",
                    VehicleId = 7,
                    Vin = "VWVZZZ1KZ1P447788"
                },
                 //Expiring Today Vignette
                 new VehicleListExpireVignetteServiceModel
                {
                    ExpireDate =  DateTime.UtcNow,
                    PlateNumber = "CT5555AM",
                    VehicleId = 8,
                    Vin = "VWVZZZ1KZ1P357159"
                },
                 //Expire but not asign
                 new VehicleListExpireVignetteServiceModel
                 {
                     ExpireDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                     PlateNumber = "CT75665AM",
                     VehicleId = 9,
                     Vin = "VWVZZZ1KZ1P357159"
                 },
                 //Expired after 30 days
                 new VehicleListExpireVignetteServiceModel
                 {
                     ExpireDate = DateTime.UtcNow.AddDays((((int)TimesPeriod.MonthlyDays) - 11)),
                     PlateNumber = "CT0987AM",
                     VehicleId = 12,
                     Vin = "WZWZASAAASDASDASDASDA"
                 },
                //Expired after 1 day
                new VehicleListExpireVignetteServiceModel
                 {
                     ExpireDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.Dayly)),
                     PlateNumber = "expire 1 day",
                     VehicleId = 15,
                     Vin = "expire 1 day"
                 },                   
            };

        private static List<BranchListingServiceModel> CollectionOfBranches(int branchId)
            => new List<BranchListingServiceModel>()
            {
                new BranchListingServiceModel
                {
                    Id = branchId,
                    Town = "Стара Загора",
                    Address = "бул. Никола Петков 55"
                }
            };

        private static List<VehicleListExpireInsurancePolicyServiceModel> CollectionOfInsurancePolicy()
            => new List<VehicleListExpireInsurancePolicyServiceModel>()
            {
                new VehicleListExpireInsurancePolicyServiceModel
                {
                    PlateNumber = "CTCTCT",
                    VehicleId = 1,
                    Vin = "Test",
                    InsurancePolicies = new List<InsurancePolicy>
                    {
                        //ExpiredFullCascoInsurance
                         new InsurancePolicy {
                             Id = 1,
                             TypeInsurance = TypeInsurance.FullCasco,
                             StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                             EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                             Expired = true,
                             InsuranceCompanyId = 1,
                             VehicleId = 1
                         },
                         //Expired ThirdPartyLiability Insurance but not asign
                         new InsurancePolicy {
                             Id = 3,
                             TypeInsurance = TypeInsurance.ThirdPartyLiability,
                            StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                            EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                            Expired = false,
                            InsuranceCompanyId = 2,
                            VehicleId = 1
                        },
                    }
                }
            };

        private static List<VehicleListingChangeOilServiceModel> CollectionOfVehiclesWithExpireOilDistance()
            => new List<VehicleListingChangeOilServiceModel>()
                {
                    new VehicleListingChangeOilServiceModel
                    {
                        VehicleId = 2,
                        PlateNumber = "CTTTTTBM",
                        EndOilChange = 180999,
                        Vin = "VWVZZZ1KZ1P111222",
                    },
                    new VehicleListingChangeOilServiceModel
                    {
                        VehicleId = 6,
                        PlateNumber = "CT9876AM",
                        EndOilChange = 198100,
                        Vin = "VWVZZZ1KZ1P98744",
                    }
            };

        private static void SetupMockObjects(Mock<IBranchesService> mockBranchService, Mock<IInsurancesPoliciesService> mockInsuranceService, Mock<IVignettesService> mockVignettesService)
        {
            if (mockBranchService != null)
            {
                mockBranchService.Setup(srv => srv.AllAsync());
            }

            if (mockInsuranceService != null)
            {
                mockInsuranceService.Setup(srv => srv.SetExpiredInsurancePoliciesLogicAsync());
            }

            if (mockVignettesService != null)
            {
                mockVignettesService.Setup(srv => srv.SetVignetteExpireLogicAsync());
            }
        }

        private static Mock<IBackgroundJobClient> GetHangFire()
        {
            return new Mock<IBackgroundJobClient>();
        }

        private static Mock<IRecurringJobManager> GetMockRecurring()
        {
            return new Mock<IRecurringJobManager>();
        }

        private static Mock<IVignettesService> GetMockVignettesService()
        {
            return new Mock<IVignettesService>();
        }

        private static Mock<IVehicleService> GetMockVehicleService()
        {
            return new Mock<IVehicleService>();
        }

        private static Mock<IInsurancesPoliciesService> GetMockInsuranceService()
        {
            return new Mock<IInsurancesPoliciesService>();
        }

        private static Mock<IBranchesService> GetMockBranchService()
        {
            return new Mock<IBranchesService>();
        }
    }
}
