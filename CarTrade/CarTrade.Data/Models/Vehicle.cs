using CarTrade.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Data.Models
{
    public class Vehicle
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(VehicleModelTypeMinLength)]
        [MaxLength(VehicleModelTypeMaxLength)]
        public string Model { get; set; }

        [Required]
        [MinLength(PlateNumberMinLength)]
        [MaxLength(PlateNumberМaxLength)]
        public string PlateNumber { get; set; }

        [MaxLength(DescriptionLength)]
        public string Description { get; set; }

        [Required]
        public DateTime YearОfМanufacture { get; set; }

        //Todo: Calculate  when Rent record (pickup + dropoff) miliageDistance has changed
        [Required]
        [Range(1, int.MaxValue)]
        public int TravelledDistance { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int EndOilChange { get; set; }

        [Required]
        [MinLength(VinMinLength)]
        [MaxLength(VinMaxLength)]
        public string Vin { get; set; }

        [Required]
        public VehicleStatus Status { get; set; }
                
        public int BranchId { get; set; }
               
        public virtual Branch Branch { get; set; }
                
        public int BrandId { get; set; }
                
        public virtual Brand Brand { get; set; }

        public int OwnerId { get; set; }

        public virtual Company Owner  { get; set; }

        public virtual IList<VehiclePicture> Pictures { get; set; } = new List<VehiclePicture>();

        public virtual IList<VehicleStuff> Stuff { get; set; } = new List<VehicleStuff>();

        public virtual IList<Rental> Rentals { get; set; } = new List<Rental>();

        //TODO: apply migration when car status is total damage;
        public virtual IList<SpareParts> Parts { get; set; } = new List<SpareParts>();
    }
}
