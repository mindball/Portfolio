using AutoMapper;
using CarTrade.Data.Enums;
using CarTrade.Data.Models;
using CarTrade.Services.InsurancePolicies;
using CarTrade.Services.InsurancePolicies.Models;
using CarTrade.Services.Tests.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Services.Tests.InsurancePolicies
{
    [Collection("Database collection")]
    public class InsurancePolicesTest : Common.Data
    {
        private DatabaseFixture fixture;
        private IMapper mapper;        
        private IInsurancesPoliciesService insuranceServices;        

        public InsurancePolicesTest(DatabaseFixture dbContext)
        {
            this.fixture = dbContext;
            this.mapper = this.fixture.MapperConfig.CreateMapper();
            this.insuranceServices = new InsurancesPoliciesService(this.fixture.Context, mapper);
        }

        [Theory]
        [InlineData("2022-01-01 00:00:00.0000000", "2021-01-01 00:00:00.0000000")]
        [InlineData("2021-01-01 00:00:00.0000000", "2021-01-01 00:00:00.0000000")]
        [InlineData("2021-12-31 00:00:00.0000001", "2021-12-31 00:00:00.0000000")]
        [InlineData("2022-01-01 00:00:00.0000000", "2021-12-31 23:59:59.9999999")]       
        public async Task AddPolicyAsyncShouldReturnErrorIfStartDateIsBiggerOrEqualEndDateAndLessThanOneYear(string startDate, string endDate) 
        {
            //Arrange           
            var model = new InsurancePolicyFormServiceModel
            {
                StartDate = DateTime.Parse(startDate),
                EndDate = DateTime.Parse(endDate)
            };

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(() 
                =>  this.insuranceServices.AddPolicyAsync(0, model));

            //Assert
            Assert.Equal(WrongDateExceptionMessage, exceptionMessage.Message);
        }

        [Fact]
        public async Task AddPolicyAsyncShouldReturnErrorIfInsuranceCompanyDoesnExist()
        {
            //Arrange
            
            var model = new InsurancePolicyFormServiceModel
            {
                StartDate = DateTime.Parse("2021-01-01 00:00:00.0000000"),
                EndDate = DateTime.Parse("2022-01-01 00:00:00.0000000"),
                InsuranceCompanyId =  int.MaxValue
            };

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(() 
                => this.insuranceServices.AddPolicyAsync(0, model));

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Message);
        }

        [Theory]
        [InlineData(TypeInsurance.FullCasco)]
        [InlineData(TypeInsurance.ThirdPartyLiability)]
        public async Task AddPolicyAsyncShouldReturnErrorIfVehicleHasActiveInsurance(TypeInsurance typeInsurance)
        {
            //Arrange          

            var model = new InsurancePolicyFormServiceModel
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.Parse("2022-01-01 00:00:00.0000000"),
                InsuranceCompanyId = 2,
                TypeInsurance = typeInsurance
            };

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(() 
                => this.insuranceServices.AddPolicyAsync(1, model));

            //Assert
            Assert.Equal(ExistItemExceptionMessage, exceptionMessage.Message);
        }

        [Fact]
        public async Task EditPolicyAsyncShoudReturnErrorIfNotExist()
        {
            //Arrange
           
            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(() 
                => this.insuranceServices.EditPolicyAsync(int.MaxValue, null));

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Message);
        }

        [Theory]
        [InlineData("2022-01-01 00:00:00.0000000", "2021-01-01 00:00:00.0000000")]
        [InlineData("2021-01-01 00:00:00.0000000", "2021-01-01 00:00:00.0000000")]
        [InlineData("2021-12-31 00:00:00.0000001", "2021-12-31 00:00:00.0000000")]
        [InlineData("2022-01-01 00:00:00.0000000", "2021-12-31 23:59:59.9999999")] 
        public async Task EditPolicyAsyncShoudReturnErrorIfDatesIsNotValid(string startDate, string endDate)
        {
            //Arrange
            
            var model = new InsurancePolicyFormServiceModel
            {
                StartDate = DateTime.Parse(startDate),
                EndDate = DateTime.Parse(endDate),               
            };

            //Act
            foreach (var item in await this.fixture.AllInsurancePoliciesAsync())
            {
                var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(() 
                    => this.insuranceServices.EditPolicyAsync(item.Id, model));

                //Assert
                Assert.Equal(WrongDateExceptionMessage, exceptionMessage.Message);
            }
        }

        [Fact]
        public async Task EditPolicyAsyncShoudEditSucessfullyPolicy()
        {
            const int policyId = 2;
            //Arrange
           
            var editingPolicy = new InsurancePolicyFormServiceModel
            {
                StartDate = DateTime.Parse("2021-12-31 23:59:59.9999999"),
                EndDate = DateTime.Parse("2022-01-01 00:00:00.0000000"),
                Expired = false,
                InsuranceCompanyId = 4,
                TypeInsurance = TypeInsurance.FullCasco,
            };

            //Act
            ////Active FullCasco insurance
            await this.insuranceServices.EditPolicyAsync(policyId, editingPolicy);
            var editedPolicy = await this.fixture
                .Context
                .InsurancePolicies
                .FirstOrDefaultAsync(i => i.Id == policyId);

            //Assert
            Assert.Equal(editingPolicy.StartDate, editedPolicy.StartDate);
            Assert.Equal(editingPolicy.EndDate, editedPolicy.EndDate);
            Assert.Equal(editingPolicy.Expired, editedPolicy.Expired);
            Assert.Equal(editingPolicy.InsuranceCompanyId, editedPolicy.InsuranceCompanyId);
            Assert.Equal(editingPolicy.TypeInsurance, editedPolicy.TypeInsurance);
        }

        [Theory, MemberData(nameof(ExpireDates))]        
        public async Task SetExpiredInsurancePoliciesLogicAsyncShouldSetAllExpiredInsuranceTrue(DateTime endDate, bool expired = false)
        {
            //Arrange
            var newInsurance = new InsurancePolicy
            {               
                TypeInsurance = TypeInsurance.FullCasco,
                StartDate = DateTime.MinValue,
                EndDate = endDate,
                Expired = expired,
                InsuranceCompanyId = 1,
                VehicleId = int.MaxValue
            };

            //Act
            await this.fixture.Context.InsurancePolicies.AddAsync(newInsurance);
            await this.fixture.Context.SaveChangesAsync();

            await insuranceServices.SetExpiredInsurancePoliciesLogicAsync();
            newInsurance = await this.fixture.Context
                .InsurancePolicies.FirstOrDefaultAsync(i => i.VehicleId == int.MaxValue);
            //Assert
            Assert.True(newInsurance.Expired);
        }
    }
}
