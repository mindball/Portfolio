using AutoMapper;
using CarTrade.Data.Enums;
using CarTrade.Services.InsurancePolicies;
using CarTrade.Services.InsurancePolicies.Models;
using System;
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
    }
}
