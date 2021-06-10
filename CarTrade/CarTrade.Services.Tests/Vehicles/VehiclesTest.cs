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

namespace CarTrade.Services.Tests.Vehicles
{
    [Collection("Database collection")]
    public class VehiclesTest : Common.Data
    {
        DatabaseFixture fixture;
        private IVehicleService vehicleService;

        public VehiclesTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            this.vehicleService = new VehicleService(this.fixture.Context, this.fixture.Mapper);
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

        //TODO: automapper test mapper.map

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
    }
}
