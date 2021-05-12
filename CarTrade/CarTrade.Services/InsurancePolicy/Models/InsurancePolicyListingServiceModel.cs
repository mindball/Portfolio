using CarTrade.Common.Mapping;
using CarTrade.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CarTrade.Services.InsurancePolicy.Models
{
    public class InsurancePolicyListingServiceModel : IMapFrom<CarTrade.Data.Models.InsurancePolicy>
    {
        public int Id { get; set; }
        
        public TypeInsurance TypeInsurance { get; set; }
        
        public DateTime StartDate { get; set; }

        public bool? Expired { get; set; }

        public DateTime EndDate { get; set; }

        public int InsuranceCompanyId { get; set; }        

        public int VehicleId { get; set; }       
    }
}
