using CarTrade.Services.Vehicle.Models;
using System.Collections.Generic;

namespace CarTrade.Web.Models.Home
{
    public class ListExpireDataForAllBranchesViewModel
    {
        public string FullAddress { get; set; }

        public IEnumerable<VehicleListExpireInsurancePolicyServiceModel> VehiclesWithExpirePolicy { get; set; }

        public IEnumerable<VehicleListExpireVignetteServiceModel> VehiclesWithExpireVignettes { get; set; }

        public IEnumerable<VehicleListingChangeOilServiceModel> VehiclesWithOilChangeDistance { get; set; }
    }
}
