using CarTrade.Data.Models;
using CarTrade.Services.Tests.Enums;
using CarTrade.Services.Vignettes;
using CarTrade.Services.Vignettes.Models;
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
        public async Task AddVignetteAsyncShouldReturnErrorMessageWhenVehicleNotExist()
        {
            //Arrange  
            var model = new VignetteFormServiceModel();

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(() => this.vignetteServise.AddVignetteAsync(int.MaxValue, model));

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Message);
        }

        [Fact]
        public async Task AddVignetteAsyncShouldReturnErrorMessageWhenVehicleHasActiveVignette()
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
        public async Task EditVignetteAsyncShouldReturnErrorIfNotExist()
        {
            //Arrange

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(()
                => this.vignetteServise.EditAsync(int.MaxValue, null));

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Message);
        }

        [Fact]
        public async Task EditVignetteAsyncShouldEditCorrectly()
        {
            //Arrange
            var editVignetteFormServiceModel = new VignetteFormServiceModel
            {
                StartDate = DateTime.UtcNow.AddDays((int)TimesPeriod.Dayly),
                EndDate = DateTime.UtcNow.AddDays((int)TimesPeriod.YearDays + (int)TimesPeriod.Dayly),
                Expired = false
            };

            var newVignette = this.fixture.Mapper
                 .Map<VignetteFormServiceModel, Data.Models.Vignette>(editVignetteFormServiceModel, opt =>
                     opt.ConfigureMap()
                     .ForMember(m => m.Id, opt => opt.Ignore())
                     .ForMember(m => m.Vehicle, opt => opt.Ignore()));

            //Act
            await this.vignetteServise.EditAsync(2, editVignetteFormServiceModel);
            var vignetteModel = await this.vignetteServise.GetByIdAsync<VignetteFormServiceModel>(2);
            //Assert
            Assert.Equal(vignetteModel.StartDate, editVignetteFormServiceModel.StartDate);
            Assert.Equal(vignetteModel.EndDate, editVignetteFormServiceModel.EndDate);
            Assert.Equal(vignetteModel.Expired, editVignetteFormServiceModel.Expired);
            
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
        [InlineData(16)]
        [InlineData(17)]        
        public async Task GetByIdAsyncShouldReturnCorrectllyModel(int expected)
        {
            // Arrange

            //Act            
            var actualModel = await this.vignetteServise.GetByIdAsync<VignetteListingServiceModel>(expected);
            var actual = actualModel.Id;

            //Assert
            Assert.Equal(expected, actual);
            Assert.NotNull(actualModel);          
        }

        [Fact]
        public async Task GetVignetteByVehicleAsyncShouldReturnErrorIfNotExist()
        {
            //Arrange

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(()
                => this.vignetteServise.GetVignetteByVehicleIdAsync<object>(int.MaxValue));

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Message);
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 2)]
        [InlineData(5, 1)]       
        public async Task GetVignetteByVehicleIdAsyncShouldReturnCorrectly(int vehicleId, int expected)
        {
            //Arrange  

            //Act
            var actualModel = await this.vignetteServise
                .GetVignetteByVehicleIdAsync<VignetteListingServiceModel>(vehicleId);
            var actual = actualModel.Count();
            //Assert
            Assert.Equal(expected, actual);
        }
    
        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(5, true)]
        [InlineData(7, false)]
        [InlineData(9, false)]
        [InlineData(10, false)]
        [InlineData(11, false)]
        public async Task DoesVehicleHaveActiveVignetteAsyncShouldTrueWhenVehicleHasActiveVignetteAndFalseWhenItNot(int vehicleId, bool expected)
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

        [Theory]
        [InlineData(1, 2)]
        [InlineData(3, 1)]        
        public async Task GetVignetteByVehicleAsyncShouldReturnCollection(int vehicleId, int expectedCount)
        {
            //Arrange 

            //Act
            var vignettes = await this.vignetteServise
                .GetVignetteByVehicleIdAsync<VignetteListingServiceModel>(vehicleId);

            //Assert
            Assert.Equal(expectedCount, vignettes.Count());
        }

        [Fact]
        public async Task GetVignetteByVehicleAsyncShouldReturnNotExistItem()
        {
            //Arrange

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(()
                => this.vignetteServise
                .GetVignetteByVehicleIdAsync<VignetteListingServiceModel>(11));            

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Message);

        }
    }
}
