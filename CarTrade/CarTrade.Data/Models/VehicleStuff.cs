using CarTrade.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace CarTrade.Data.Models
{
    public class VehicleStuff
    {
        public int Id { get; set; }

        [Required]
        public Stuff Stuff { get; set; }

        public DateTime? ExpireDate { get; set; }

        public int VehicleId { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
