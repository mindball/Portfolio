using AutoMapper;
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
        private IMapper mapper;

        private static bool IsFilledVignettes = false;
        private IVignettesService vignetteServide;

        public VignettesTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            this.mapper = this.fixture.MapperConfig.CreateMapper();
            this.vignetteServide = new VignettesService(this.fixture.Context, mapper);
        }

        [Fact]
        public async Task AddVignetteAsyncShoudReturnErrorMessageWhenVehicleNotExist()
        {
            //Arrange  
            var model = new VignetteFormServiceModel();

            //Act
            var exceptionMessage = Assert.ThrowsAsync<ArgumentException>(() => this.vignetteServide.AddVignetteAsync(int.MaxValue, model));

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Result.Message);
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
                var exceptionMessage = Assert.ThrowsAsync<ArgumentException>(() => this.vignetteServide.AddVignetteAsync(activeVignette.VehicleId, null));
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
                => this.vignetteServide.AddVignetteAsync(10, newVignetteModel));

            //Assert
            Assert.Equal(WrongDateExceptionMessage, exceptionMessage.Message);
        }
    }
}
