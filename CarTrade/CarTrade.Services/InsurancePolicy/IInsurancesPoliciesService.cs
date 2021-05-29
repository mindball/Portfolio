using CarTrade.Data.Enums;
using CarTrade.Services.InsurancePolicy.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.InsurancePolicy
{
    public interface IInsurancesPoliciesService
    {
        Task AddPolicyAsync(int vehicleId, InsurancePolicyFormServiceModel newPolicy);

        Task EditPolicyAsync(int insuranceId, InsurancePolicyFormServiceModel insurancePoliceModel);

        Task<TModel> GetByIdAsync<TModel>(int insuranceId) where TModel : class;

        Task<IEnumerable<InsurancePolicyListingServiceModel>> GetAllInsuranceByVehicleId(int vehicleId);

        Task SetExpiredInsurancePoliciesLogicAsync();
    }
}
