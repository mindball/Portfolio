using AutoMapper;
using CarTrade.Data.Models;
using CarTrade.Services.Vignettes;
using CarTrade.Services.Vignettes.Models;
using CarTrade.Web.Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Services.Tests.Vignettes
{
    [Collection("Database collection")]
    public class VignettesTest : Common.Data
    {
        DatabaseFixture fixture;
        private IVignettesService vignetteServise;

        public VignettesTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;           
            this.vignetteServise = new VignettesService(this.fixture.Context, this.fixture.Mapper);
        }

        [Fact]
        public async Task AddVignetteAsyncShoudReturnErrorMessageWhenVehicleNotExist()
        {
            //Arrange  
            var model = new VignetteFormServiceModel();

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(() => this.vignetteServise.AddVignetteAsync(int.MaxValue, model));

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Message);
        }

        [Fact]
        public async Task AddVignetteAsyncShoudReturnErrorMessageWhenVehicleHasActiveVignette()
        {
            //Arrange  
            //var existVignette = this.fixture
            //    .Context.Vignettes
            //    .Where(v => !v.Expired && v.EndDate > DateTime.UtcNow)
            //    .ToList();

            //Act
            var existVignette = await this.fixture
                .Context.Vignettes
                .Where(v => v.EndDate > DateTime.UtcNow)
                .ToListAsync();

            //Assert
            foreach (var activeVignette in existVignette)
            {
                var exceptionMessage = Assert.ThrowsAsync<ArgumentException>(() => this.vignetteServise.AddVignetteAsync(activeVignette.VehicleId, null));
                Assert.Equal(ExistItemExceptionMessage, exceptionMessage.Result.Message);
            }
        }

        [Theory, MemberData(nameof(StartDateBiggerThanEndDateData))]
        public async Task AddVignetteAsyncShouldReturnErrorIfStartDateIsBiggerOrEqualEndDate(DateTime startDate, DateTime endDate)
        {
            //Arrange  
            var newVignetteModel = new VignetteFormServiceModel
            {
                Expired = false,
                StartDate = startDate,
                EndDate = endDate
            };

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(()
                => this.vignetteServise.AddVignetteAsync(10, newVignetteModel));

            //Assert
            Assert.Equal(WrongDateExceptionMessage, exceptionMessage.Message);
        }

        [Fact]
        public async Task EditVignetteAsyncShoudReturnErrorIfNotExist()
        {
            //Arrange

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(()
                => this.vignetteServise.EditAsync(int.MaxValue, null));

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Message);
        }

        [Fact]
        public async Task GetByIdAsyncShouldReturnErrorIfNotExist()
        {
            // Arrange

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(()
               => this.vignetteServise.GetByIdAsync<object>(int.MaxValue));

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Message);
        }

        [Fact]
        public async Task GetVignetteByVehicleAsyncShoudReturnErrorIfNotExist()
        {
            //Arrange

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(()
                => this.vignetteServise.GetVignetteByVehicleIdAsync<object>(int.MaxValue));

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Message);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(5, true)]
        [InlineData(7, false)]
        [InlineData(9, false)]
        [InlineData(10, false)]
        [InlineData(11, false)]
        public async Task DoesVehicleHaveActiveVignetteAsyncShoudTrueWhenVehicleHasActiveVignetteAndFalseWhenItNot(int vehicleId, bool expected)
        {
            //Arrange  

            //Act
            var isActiveVignetteResult = await this.vignetteServise.DoesVehicleHaveActiveVignetteAsync(vehicleId);

            //Assert
            Assert.Equal(expected, isActiveVignetteResult);
        }

        [Fact]
        public async Task SetVignetteExpireLogicAsyncShouldSetAllExpiredInsuranceTrue()
        {
            //Arrange

            //Act
            var expectetResultVignettesWithExpiredEndDate =
                (await this.fixture.Context
                .Vignettes
                .Where(vg =>
                    vg.Expired == false
                    && vg.EndDate <= DateTime.UtcNow)
               .ToListAsync())
               .Count();

            var actualResultVignettesWithExpiredEndDate =
                await this.vignetteServise.SetVignetteExpireLogicAsync();

            //Assert
            Assert.Equal(expectetResultVignettesWithExpiredEndDate,
                actualResultVignettesWithExpiredEndDate);
        }

        //[Theory]
        //[InlineData(1, 2)]
        //[InlineData(3, 1)]
        //[InlineData(11, 0)]
        //public async Task GetVignetteByVehicleAsyncShoudReturnCollection(int vehicleId, int expectedCount)
        //{
        //    //Arrange  
        //    this.fixture.MapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        //    this.vignetteServise = new VignettesService(this.fixture.Context, new Mapper(this.fixture.MapperConfig));

        //    //Act
        //    var vignettes = await this.vignetteServise
        //        .GetVignetteByVehicleIdAsync<VignetteListingServiceModel>(vehicleId);

        //    //Assert
        //    Assert.Equal(expectedCount, vignettes.Count());            
        //}

        //[Fact]
        //public async Task GetVignetteByVehicleAsyncShoudReturnEmptyCollection()
        //{
        //    //Arrange

        //    //Act
        //    var vignettes = await this.vignetteServise
        //        .GetVignetteByVehicleIdAsync<VignetteListingServiceModel>(11);

        //    //Assert
        //    Assert.Equal(2, vignettes.Count());

        //}
    }
}
