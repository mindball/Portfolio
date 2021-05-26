using System;

namespace CarTrade.Web.Models.Vignettes
{
    public class VignetteDetailViewModel
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int VehicleId { get; set; }
    }
}
