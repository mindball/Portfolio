using CarTrade.Services.Vehicles.Models;
using System;
using System.Collections.Generic;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Web.Models.Vehicles
{
    public class VehicleListingViewModel
    {
        public IEnumerable<VehicleListingServiceModel> Vehicles { get; set; }

        public int TotalVehicles { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)this.TotalVehicles / VehiclePageSize);

        public int CurrentPage { get; set; }

        public int PreviousPage => this.CurrentPage <= 1 ? 1 : this.CurrentPage - 1;

        public int NextPage
            => this.CurrentPage == this.TotalPages
                ? this.TotalPages
                : this.CurrentPage + 1;
    }
}
