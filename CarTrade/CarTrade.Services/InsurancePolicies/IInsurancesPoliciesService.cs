using CarTrade.Data.Enums;
using CarTrade.Services.InsurancePolicies.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.InsurancePolicies
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
