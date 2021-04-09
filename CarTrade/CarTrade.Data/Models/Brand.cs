using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Data.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required]
        [MinLength(VehicleModelTypeMinLength)]
        [MaxLength(VehicleModelTypeMaxLength)]
        public string Name { get; set; }

        public virtual IList<Vehicle> Cars { get; set; } = new List<Vehicle>();
    }
}
