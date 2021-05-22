using CarTrade.Services.Vehicle.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CarTrade.Web.Models.Home
{
    public class ListExpireDataForAllBranchesViewModel
    {
        public int BranchId { get; set; }

        public string FullAddress { get; set; }
        
        public IList<VehicleListExpireInsurancePolicyServiceModel> VehiclesWithExpirePolicy { get; set; }

        public IList<VehicleListExpireVignetteServiceModel> VehiclesWithExpireVignettes { get; set; }

        public IList<VehicleListingChangeOilServiceModel> VehiclesWithOilChangeDistance { get; set; }
        public IList<VehicleListingInspectionSafetyCheckServiceModel> VehiclesWithInspectionExpire{ get; set; }
    }
}
