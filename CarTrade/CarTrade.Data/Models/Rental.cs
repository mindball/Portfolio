using System;
using System.ComponentModel.DataAnnotations;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Data.Models
{
    public class Rental
    {
        public int Id { get; set; }

        [Required]
        public DateTime PickUp { get; set; }

        //TODO: Logic case when record Pickup, Dropoff must be required or not
        //[Required]
        public DateTime? DropOff { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StartMileage { get; set; }
        
        [Range(0, int.MaxValue)]
        public int? EndMileage { get; set; }

        [MaxLength(DescriptionLength)]
        public string Description { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }

        public int VehicleId { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
