using CarTrade.Services.Vehicles;
using CarTrade.Services.Vehicles.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

using static CarTrade.Common.DataConstants;
using CarTrade.Data.Enums;
using CarTrade.Services.Tests.Enums;

namespace CarTrade.Services.Tests.Vehicles
{
    [Collection("Database collection")]
    public class VehiclesTest : Common.Data
    {
        private DatabaseFixture fixture;
        private IVehicleService vehicleService;

        public VehiclesTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            this.vehicleService = new VehicleService(this.fixture.Context, this.fixture.Mapper);
        }

        [Fact]
        public async Task AllAsyncShouldReturnCorrectrlyCount()
        {
            //Arrange
            int expected = 15;

            //Act
            var actual = (await this.vehicleService.AllAsync()).Count();

            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory, MemberData(nameof(ValidateVehicleFormServiceModel))]
        public async Task AddVehicleAsyncReturnErrorWhenValidateVehicleServiceModelFail(VehicleFormServiceModel vehicleModel)
        {
            //Arrange

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(()
                => this.vehicleService.AddVehicleAsync(vehicleModel));

            //Assert
            Assert.Equal(ValidateItemException, exceptionMessage.Message);
        }

        [Fact]
        public async Task AddVehicleAsyncShouldSuccessfullyAddVehicle()
        {
            //Arrange
            var newVehicle = new VehicleFormServiceModel
            {
                Model = "Q7",
                PlateNumber = "CTCTCTCT",
                YearОfМanufacture = DateTime.UtcNow.AddYears(5),
                TravelledDistance = 198000,
                EndOilChange = 208000,
                Vin = "VVVVVWWWWZZZZZAAAAA",
                Status = VehicleStatus.OnMotion,
                InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.HalfYearDays),
                BranchId = 4,
                BrandId = 1,
                OwnerId = 1,
            };

            //Act
            var lenBeforeAddEnitityOfInMemoryDB = await this.fixture.Context.Vehicles.CountAsync();
            await this.vehicleService.AddVehicleAsync(newVehicle);
            var lenAfterAddEnitityOfInMemoryDB = await this.fixture.Context.Vehicles.CountAsync();
            var index = lenAfterAddEnitityOfInMemoryDB - 1;
            var actual = (await this.vehicleService.AllAsync()).Count();
            var lastAddVehicle = await this.fixture.Context.Vehicles.FirstOrDefaultAsync(x => x.Id == index);

            //Assert
            Assert.Equal(lenAfterAddEnitityOfInMemoryDB, actual);
            Assert.Equal(index, lastAddVehicle.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        public async Task EditVehicleAsyncReturnErrorWhenVehicleNotExist(int vehicleId)
        {
            //Arrange

            //Act
            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(()
                => this.vehicleService.EditVehicleAsync(vehicleId, null));

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Message);
        }
                
        [Fact]
        public async Task EditVehicleAsyncShouldEditCorecttly()
        {
            //Arrange
            var newVehicle = new VehicleFormServiceModel
            {
                Model = "Q7",
                PlateNumber = "CTCTCTCT",
                YearОfМanufacture = DateTime.UtcNow.AddYears(5),
                TravelledDistance = 198000,
                EndOilChange = 208000,
                Vin = "VVVVVWWWWZZZZZAAAAA",
                Status = VehicleStatus.OnMotion,
                InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.HalfYearDays),
                BranchId = 4,
                BrandId = 1,
                OwnerId = 1,
            };

            //Act            
            await this.vehicleService.EditVehicleAsync(15, newVehicle);
            var editVehicle = await this.vehicleService.GetByIdAsync<VehicleFormServiceModel>(15);


            //Assert
            Assert.Equal(newVehicle.Model, editVehicle.Model);
            Assert.Equal(newVehicle.PlateNumber, editVehicle.PlateNumber);
            Assert.Equal(newVehicle.YearОfМanufacture, editVehicle.YearОfМanufacture);
            Assert.Equal(newVehicle.EndOilChange, editVehicle.EndOilChange);
            Assert.Equal(newVehicle.Status, editVehicle.Status);
            Assert.Equal(newVehicle.InspectionSafetyCheck, editVehicle.InspectionSafetyCheck);
            Assert.Equal(newVehicle.BranchId, editVehicle.BranchId);
            Assert.Equal(newVehicle.BrandId, editVehicle.BrandId);
            Assert.Equal(newVehicle.OwnerId, editVehicle.OwnerId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
       
        public async Task GetByIdAsyncShouldReturnCorrectlyEntity(int vehicleId)
        {
            //Arrange

            //Act
            var actual = await this.vehicleService.GetByIdAsync<VehicleListingServiceModel>(vehicleId);
            //Assert
            Assert.NotNull(actual);
            Assert.Equal(vehicleId, actual.Id);
        }

        [Theory]
        [InlineData(1)]        
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        public async Task GetInsuranceExpireDataAsyncFetchCollection(int branchId)
        {
            //Arrange
            //Expir(ed)ing insuranceId = 3, 8, 9, 10, 11, 12
            // vehicleId =               1, 5, 2, 5,  3,  8
            //branchId                   1, 1, 2, 1,  3,  1
            var expected = await this.fixture.Context.Vehicles
                .Where(b => b.BranchId == branchId &&
                        b.InsurancePolicies
                        .Any(i => i.EndDate <= DateTime.UtcNow.AddDays(DaysBeforeItExpires)))
                 .Select(v => new VehicleListExpireInsurancePolicyServiceModel
                 {
                     VehicleId = v.Id,
                     PlateNumber = v.PlateNumber,
                     Vin = v.Vin,
                     InsurancePolicies = v.InsurancePolicies
                                .Where(i => i.EndDate <= DateTime.UtcNow.AddDays(DaysBeforeItExpires))
                                .Select(i => i)
                                .ToList()
                 })
                 .ToListAsync();

            //Act
            var actual = await this.vehicleService.GetInsuranceExpireDataAsync(branchId);

            //Assert
            Assert.NotNull(actual);
            expected.Should().BeEquivalentTo(actual);
            Assert.Equal(expected.Count(), actual.Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        public async Task GetVignetteExpireDataAsyncFetchCollection(int branchId)
        {
            //Arrange
            //Expir(ed)ing vignetteID = 9, 10, 11, 13, 14, 15, 16, 17
            //Expir(ed)ing vehicleId =  7,  8,  9, 10, 12, 13, 14, 15  
            //branchId                  1,  1,  3,  3,  5,  5,  5,  5
            var expected = await this.fixture.Context.Vehicles
                .Where(v => v.BranchId == branchId
                    && v.Vignettes.Any(vg => vg.VehicleId == v.Id 
                                && !vg.Expired 
                                && vg.EndDate <= DateTime.UtcNow.AddDays(DaysBeforeItExpires)))
                 .Select(v => new VehicleListExpireVignetteServiceModel
                 {
                     VehicleId = v.Id,
                     PlateNumber = v.PlateNumber,
                     Vin = v.Vin,
                     ExpireDate = v.Vignettes.Where(vh => vh.VehicleId == v.Id).Select(vg => vg.EndDate).FirstOrDefault()
                 })
                 .ToListAsync();

            //Act
            var actual = await this.vehicleService.GetVignetteExpireDataAsync(branchId);

            //Assert
            Assert.NotNull(actual);
            expected.Should().BeEquivalentTo(actual);
            Assert.Equal(expected.Count(), actual.Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(int.MaxValue)]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetOilChangeExpireDataAsyncFetchCollection(int branchId)
        {
            //Arrange            
            //Chang(ed)ing vehicleId =  1, 2, 6, 7, 8, 9, 10
            //branchId                  1, 2, 4, 1, 1, 3,  3,
            var expected = await this.fixture.Context.Vehicles
                .Where(v => v.BranchId == branchId
                        && (v.TravelledDistance + RemainDistanceOilChange) >= v.EndOilChange)
                .Select(v => new VehicleListingChangeOilServiceModel
                {
                    VehicleId = v.Id,
                    PlateNumber = v.PlateNumber,
                    Vin = v.Vin,
                    EndOilChange = v.EndOilChange
                })
                .ToListAsync();

            //Act
            var actual = await this.vehicleService.GetOilChangeExpireDataAsync(branchId);

            //Assert
            Assert.NotNull(actual);
            expected.Should().BeEquivalentTo(actual);
            Assert.Equal(expected.Count(), actual.Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(int.MaxValue)]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetInspectionSafetyCheckExpireDataAsyncFetchCollection(int branchId)
        {

            //Arrange            
            //Chang(ed)ing vehicleId =  3, 4, 5, 6, 7, 10, 12
            //branchId                  3, 1, 1, 4, 1,  3, 5
            var expected = await this.fixture.Context.Vehicles
                .Where(v => v.BranchId == branchId
                    && (v.InspectionSafetyCheck <= DateTime.UtcNow.AddDays(DaysBeforeItExpires)))
            .Select(v => new VehicleListingInspectionSafetyCheckServiceModel
            {
                VehicleId = v.Id,
                PlateNumber = v.PlateNumber,
                Vin = v.Vin,
                InspectionSafetyCheck = v.InspectionSafetyCheck
            })
            .ToListAsync();

            //Act
            var actual = await this.vehicleService.GetInspectionSafetyCheckExpireDataAsync(branchId);

            //Assert
            Assert.NotNull(actual);
            expected.Should().BeEquivalentTo(actual);
            Assert.Equal(expected.Count(), actual.Count());
        }
    }
}
