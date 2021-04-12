using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarTrade.Data.Models
{
    public class Branch
    {
        public int Id { get; set; }

        [Required]
        public string Town { get; set; }

        public string Address { get; set; }

        public virtual IList<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        public virtual IList<User> Users { get; set; } = new List<User>();
    }
}
