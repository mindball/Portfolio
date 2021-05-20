using System;
using System.Collections.Generic;
using System.Text;

namespace CarTrade.Services.Vehicle.Models
{
    public class VehicleListExpireVignetteServiceModel : VehicleBasicListingServiceModel
    {
        public DateTime ExpireDate { get; set; }
    }
}
