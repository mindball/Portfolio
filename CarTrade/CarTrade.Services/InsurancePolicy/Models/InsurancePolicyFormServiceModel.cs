using CarTrade.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace CarTrade.Services.InsurancePolicy.Models
{
    public class InsurancePolicyFormServiceModel
    {
        [Required]
        public TypeInsurance TypeInsurance { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
                
        public bool? Expired { get; set; }

        [Required]
        public int InsuranceCompanyId { get; set; }

        //[Required]
        //public int VehicleId { get; set; }       
    }
}
