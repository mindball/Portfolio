using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Data.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [MinLength(MinNameLength)]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; }

        public virtual IList<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        public virtual IList<User> Employees { get; set; } = new List<User>();
    }
}
