using System;
using System.Collections.Generic;
using System.Text;

namespace CarTrade.Services.Vehicle.Models
{
    public class VehicleListExpireInsurancePolicyServiceModel : VehicleBasicListingServiceModel
    {
        public IEnumerable<Data.Models.InsurancePolicy> InsurancePolicies { get; set; }
    }
}
