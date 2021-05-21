using System;
using System.Collections.Generic;
using System.Text;

namespace CarTrade.Services.Vehicle.Models
{
    public class VehicleListExpireVignetteServiceModel : VehicleExpireBasicListingServiceModel
    {
        public DateTime ExpireDate { get; set; }
    }
}
