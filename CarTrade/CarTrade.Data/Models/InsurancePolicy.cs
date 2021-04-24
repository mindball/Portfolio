using CarTrade.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Data.Models
{
    public class InsurancePolicy
    {
        public int Id { get; set; }                

        [Required]
        public TypeInsurance TypeInsurance { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int InsuranceCompanyId { get; set; }

        public virtual InsuranceCompany InsuanceCompany { get; set; }

        public virtual IList<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
