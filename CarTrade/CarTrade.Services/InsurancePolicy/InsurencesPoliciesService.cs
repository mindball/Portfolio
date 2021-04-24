using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Data.Enums;
using CarTrade.Services.InsurancePolicy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CarTrade.Services.InsurancePolicy
{
    public class InsurencesPoliciesService : IInsurencesPoliciesService
    {
        private readonly CarDbContext db;

        public InsurencesPoliciesService(CarDbContext db)
        {
            this.db = db;
        }

        public async Task AddPolicyAsync(TypeInsurance type, 
            DateTime startDate, 
            DateTime endDate, 
            int insuanceCompanyId)
        {
            Type enumType = type.GetType();
            bool isEnumValid = Enum.IsDefined(enumType, type);

            if (!isEnumValid)
            {
                throw new Exception("Wrong policy type");
            }

            if (!this.CompareStartEndDate(startDate, endDate))
            {
                throw new ArgumentException("Start date must be small than end date");
            }

            var insuranceCompany = await db.InsuranceCompanies.FindAsync(insuanceCompanyId);
            if(insuranceCompany == null)
            {
                throw new ArgumentException("Missing Insurance company");
            }

            var newInsurancePolicy = new Data.Models.InsurancePolicy
            {
                TypeInsurance = type,
                StartDate = startDate,
                EndDate = endDate,
                InsuranceCompanyId = insuranceCompany.Id
            };

            await this.db.InsurancePolicies.AddAsync(newInsurancePolicy);
            await this.db.SaveChangesAsync();
        }

        public async Task EditPolicyAsync(InsurancePolicyListingServiceModel insurancePolicyModel)
        {
            var existInsurancePolicy = await this.db.InsurancePolicies.FindAsync(insurancePolicyModel.Id);
            if(existInsurancePolicy == null)
            {
                throw new ArgumentException("Missing policy");
            }

            if (!this.CompareStartEndDate(insurancePolicyModel.StartDate, insurancePolicyModel.EndDate))
            {
                throw new ArgumentException("Start date must be small than end date");
            }

            existInsurancePolicy.StartDate = insurancePolicyModel.StartDate;
            existInsurancePolicy.EndDate = insurancePolicyModel.EndDate;
            existInsurancePolicy.InsuranceCompanyId = insurancePolicyModel.InsuranceCompanyId;
            existInsurancePolicy.TypeInsurance = insurancePolicyModel.TypeInsurance;

            await this.db.SaveChangesAsync();
        }

        public async Task<InsurancePolicyListingServiceModel> GetByIdAsync(int insuranceId)
        {
            var existInsurancePolicy = 
                await this.db.InsurancePolicies
                .ProjectTo<InsurancePolicyListingServiceModel>()
                .FirstOrDefaultAsync(p => p.Id == insuranceId);

            if (existInsurancePolicy == null) throw new ArgumentException("Missing policy");

            return existInsurancePolicy;
        }

        private bool CompareStartEndDate(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                return false;
            }

            return true;
        }
    }
}
