using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarTrade.Data.Models
{
    //TODO: apply migration when car status is total damage;
    public class SparePart
    {
        public int Id { get; set; }

        public string OENumber { get; set; }

        public string PartsNumber { get; set; }

        [Required]
        public string  Name { get; set; }

        public virtual IList<VehiclesSpareParts> Parts { get; set; } = new List<VehiclesSpareParts>();
    }
}
