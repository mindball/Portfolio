using CarTrade.Data.Enums;
using CarTrade.Services.InsurancePolicy.Models;
using System;
using System.Threading.Tasks;

namespace CarTrade.Services.InsurancePolicy
{
    public interface IInsurencesPoliciesService
    {
        Task AddPolicyAsync(TypeInsurance type, DateTime startDate, DateTime endDate, int insuanceCompanyId);

        Task EditPolicyAsync(InsurancePolicyListingServiceModel insurancePoliceModel);

        Task<InsurancePolicyListingServiceModel> GetByIdAsync(int insuranceId);
    }
}
