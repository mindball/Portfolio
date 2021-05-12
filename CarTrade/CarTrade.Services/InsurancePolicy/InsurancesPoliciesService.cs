using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Data.Enums;
using CarTrade.Services.InsurancePolicy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Services.InsurancePolicy
{
    public class InsurancesPoliciesService : IInsurancesPoliciesService
    {
        private readonly CarDbContext db;

        public InsurancesPoliciesService(CarDbContext db)
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

        public async Task<IEnumerable<InsurancePolicyListingServiceModel>> GetAllInsuranceByVehicleId(int vehicleId)
            => await this.db.InsurancePolicies
            .Where(v => v.VehicleId == vehicleId)
            .ProjectTo<InsurancePolicyListingServiceModel>()
            .ToListAsync();

        public async Task<TModel> GetByIdAsync<TModel>(int insuranceId) where TModel : class
        {
            var existInsurancePolicy =
                await this.db.InsurancePolicies
                .Where(ip => ip.Id == insuranceId)
                .ProjectTo<TModel>()
                .FirstOrDefaultAsync();
                

            //TODO: make custom exception handler on controllers
            if (existInsurancePolicy == null)
            {
                return null;
                throw new ArgumentException("Missing policy");
            } 

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
