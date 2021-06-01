using CarTrade.Services.Vehicles.Models;
using System.Collections.Generic;

namespace CarTrade.Services.Branches.Models
{
    public class BranchVehiclesListingServiceModel
    {
        public string FullAddress { get; set; }

        public IEnumerable<VehicleBasicListingServiceModel> AllVehicles { get; set; }
    }
}
