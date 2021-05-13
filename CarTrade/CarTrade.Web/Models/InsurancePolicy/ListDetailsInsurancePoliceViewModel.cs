using System.Collections.Generic;

namespace CarTrade.Web.Models.InsurancePolicy
{
    public class ListDetailsInsurancePoliceViewModel
    {
        public int VehicleId { get; set; }

        public string BrandName { get; set; }

        public string VehicleModel { get; set; }

        public string PlateNumber { get; set; }

        public string Vin { get; set; }

        public IEnumerable<DetailInsurancePolicyViewModel> PolicyDetails { get; set; }
    }
}
