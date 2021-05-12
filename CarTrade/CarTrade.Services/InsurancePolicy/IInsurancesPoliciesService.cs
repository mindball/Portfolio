using CarTrade.Data.Enums;
using CarTrade.Services.InsurancePolicy.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.InsurancePolicy
{
    public interface IInsurancesPoliciesService
    {
        Task AddPolicyAsync(TypeInsurance type, DateTime startDate, DateTime endDate, int insuanceCompanyId);

        Task EditPolicyAsync(InsurancePolicyListingServiceModel insurancePoliceModel);

        Task<TModel> GetByIdAsync<TModel>(int insuranceId) where TModel : class;

        Task<IEnumerable<InsurancePolicyListingServiceModel>> GetAllInsuranceByVehicleId(int vehicleId);
    }
}
