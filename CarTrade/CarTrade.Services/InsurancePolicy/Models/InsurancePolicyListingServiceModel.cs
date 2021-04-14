using CarTrade.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CarTrade.Services.InsurancePolicy.Models
{
    public class InsurancePolicyListingServiceModel
    {
        public int Id { get; set; }

        [Required]
        public TypeInsurance TypeInsurance { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int InsuranceCompanyId { get; set; }
    }
}
