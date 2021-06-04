using AutoMapper;
using CarTrade.Data.Enums;
using CarTrade.Data.Models;
using CarTrade.Services.InsurancePolicies;
using CarTrade.Services.InsurancePolicies.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Services.Tests.InsurancePolicies
{
    public class InsurancePolicesTest
    {
        private DbContext dbContext;
        private IMapper mapper; 
        private static bool IsFilledInsurancePolicies = false;
        private static bool IsFilledInsuranceCompany = false;
        private IInsurancesPoliciesService insuranceServices;        

        public InsurancePolicesTest()
        {
            this.dbContext = new DbContext();
            this.mapper = this.dbContext.MapperConfig.CreateMapper();
            this.insuranceServices = new InsurancesPoliciesService(this.dbContext.Context, mapper);
        }

        [Theory]
        [InlineData("2022-01-01 00:00:00.0000000", "2021-01-01 00:00:00.0000000")]
        [InlineData("2021-01-01 00:00:00.0000000", "2021-01-01 00:00:00.0000000")]
        [InlineData("2021-12-31 00:00:00.0000001", "2021-12-31 00:00:00.0000000")]
        [InlineData("2022-01-01 00:00:00.0000000", "2021-12-31 23:59:59.9999999")]       
        public async Task AddPolicyAsyncShouldReturnErrorIfStartDateIsBiggerOrEqualEndDateAndLessThanOneYear(string startDate, string endDate) 
        {
            //Arrange
            if (!IsFilledInsurancePolicies)
            {
                await this.dbContext.FillInsurancePoliciesAsync();
                IsFilledInsurancePolicies = true;
            }

            var model = new InsurancePolicyFormServiceModel
            {
                StartDate = DateTime.Parse(startDate),
                EndDate = DateTime.Parse(endDate)
            };

            //Act
            var exceptionMessage = Assert.ThrowsAsync<ArgumentException>(() =>  this.insuranceServices.AddPolicyAsync(0, model));


            //Assert
            Assert.Equal(WrongDateExceptionMessage, exceptionMessage.Result.Message);
        }

        [Fact]
        public async Task AddPolicyAsyncShouldReturnErrorIfInsuranceCompanyDoesnExist()
        {
            //Arrange
            if (!IsFilledInsurancePolicies)
            {
                await this.dbContext.FillInsurancePoliciesAsync();
                IsFilledInsurancePolicies = true;
            }
            if (!IsFilledInsuranceCompany)
            {
                await this.dbContext.FillInsuranceCompanyAsync();
                IsFilledInsuranceCompany = true;
            }

            var model = new InsurancePolicyFormServiceModel
            {
                StartDate = DateTime.Parse("2021-01-01 00:00:00.0000000"),
                EndDate = DateTime.Parse("2022-01-01 00:00:00.0000000"),
                InsuranceCompanyId =  int.MaxValue
            };

            //Act
            var exceptionMessage = Assert.ThrowsAsync<ArgumentException>(() => this.insuranceServices.AddPolicyAsync(0, model));

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Result.Message);
        }

        [Theory]
        [InlineData(TypeInsurance.FullCasco)]
        [InlineData(TypeInsurance.ThirdPartyLiability)]
        public async Task AddPolicyAsyncShouldReturnErrorIfVehicleHasActiveInsurance(TypeInsurance typeInsurance)
        {
            //Arrange
            if (!IsFilledInsurancePolicies)
            {
                await this.dbContext.FillInsurancePoliciesAsync();
                IsFilledInsurancePolicies = true;
            }

            if (!IsFilledInsuranceCompany)
            {
                await this.dbContext.FillInsuranceCompanyAsync();
                IsFilledInsuranceCompany = true;
            }

            var model = new InsurancePolicyFormServiceModel
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.Parse("2022-01-01 00:00:00.0000000"),
                InsuranceCompanyId = 2,
                TypeInsurance = typeInsurance
            };

            //Act
            var exceptionMessage = Assert.ThrowsAsync<ArgumentException>(() => this.insuranceServices.AddPolicyAsync(1, model));

            //Assert
            Assert.Equal(ExistItemExceptionMessage, exceptionMessage.Result.Message);
        }

        [Fact]
        public async Task EditPolicyAsyncShoudReturnErrorIfNotExist()
        {
            //Arrange
            if (!IsFilledInsurancePolicies)
            {
                await this.dbContext.FillInsurancePoliciesAsync();
                IsFilledInsurancePolicies = true;
            }

            //Act
            var exceptionMessage = Assert.ThrowsAsync<ArgumentException>(() 
                => this.insuranceServices.EditPolicyAsync(int.MaxValue, null));

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Result.Message);
        }

        [Theory]
        [InlineData("2022-01-01 00:00:00.0000000", "2021-01-01 00:00:00.0000000")]
        [InlineData("2021-01-01 00:00:00.0000000", "2021-01-01 00:00:00.0000000")]
        [InlineData("2021-12-31 00:00:00.0000001", "2021-12-31 00:00:00.0000000")]
        [InlineData("2022-01-01 00:00:00.0000000", "2021-12-31 23:59:59.9999999")] 
        public async Task EditPolicyAsyncShoudReturnErrorIfDatesIsNotValid(string startDate, string endDate)
        {
            //Arrange
            if (!IsFilledInsurancePolicies)
            {
                await this.dbContext.FillInsurancePoliciesAsync();
                IsFilledInsurancePolicies = true;
            }

            var model = new InsurancePolicyFormServiceModel
            {
                StartDate = DateTime.Parse(startDate),
                EndDate = DateTime.Parse(endDate),               
            };

            //Act
            foreach (var item in await this.dbContext.AllInsurancePoliciesAsync())
            {
                var exceptionMessage = Assert.ThrowsAsync<ArgumentException>(() => this.insuranceServices.EditPolicyAsync(item.Id, model));

                //Assert
                Assert.Equal(WrongDateExceptionMessage, exceptionMessage.Result.Message);
            }
        }

        [Fact]
        public async Task EditPolicyAsyncShoudEditSucessfullyPolicy()
        {
            const int policyId = 2;
            //Arrange
            if (!IsFilledInsurancePolicies)
            {
                await this.dbContext.FillInsurancePoliciesAsync();
                IsFilledInsurancePolicies = true;
            }

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
            var editedPolicy = await this.dbContext
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

        [Theory, MemberData(nameof(ExpiredDates))]        
        public async Task SetExpiredInsurancePoliciesLogicAsyncShouldSetAllExpiredInsuranceTrue(DateTime endDate, bool expired)
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
            await this.dbContext.Context.InsurancePolicies.AddAsync(newInsurance);
            await this.dbContext.Context.SaveChangesAsync();

            await insuranceServices.SetExpiredInsurancePoliciesLogicAsync();
            newInsurance = await this.dbContext.Context
                .InsurancePolicies.FirstOrDefaultAsync(i => i.VehicleId == int.MaxValue);
            //Assert
            Assert.True(newInsurance.Expired);
        }

        public static IEnumerable<object[]> ExpiredDates => new[]
        {  
            new object[] { DateTime.UtcNow.AddYears(-1), false },
            new object[] { DateTime.UtcNow.AddMonths(-1), false },            
            new object[] { DateTime.UtcNow.AddDays(-1), false },
            new object[] { DateTime.UtcNow.AddHours(-1), false },
            new object[] { DateTime.UtcNow.AddMinutes(-1), false },
            new object[] { DateTime.UtcNow.AddSeconds(-1), false },
            new object[] { DateTime.UtcNow.AddMilliseconds(-1), false }
        };
    }
}
