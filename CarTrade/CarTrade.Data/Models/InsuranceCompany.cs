using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static CarTrade.Common.DataConstants;
namespace CarTrade.Data.Models
{
    public class InsuranceCompany
    {
        public int Id { get; set; }

        [Required]
        [MinLength(MinNameLength)]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; }

        public virtual IList<InsurancePolicy> InsurancePolicies { get; set; } = new List<InsurancePolicy>();
    }
}
