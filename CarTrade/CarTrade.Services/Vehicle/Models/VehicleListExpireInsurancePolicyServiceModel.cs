using CarTrade.Services.InsurancePolicies.Models;
using System.Collections.Generic;

namespace CarTrade.Services.Vehicle.Models
{
    public class VehicleListExpireInsurancePolicyServiceModel : VehicleExpireBasicListingServiceModel
    {   
        public IList<Data.Models.InsurancePolicy> InsurancePolicies { get; set; }
    }
}
