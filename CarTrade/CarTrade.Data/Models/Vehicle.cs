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

        [Required]
        public DateTime InspectionSafetyCheck { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int BranchId { get; set; }
               
        public virtual Branch Branch { get; set; }
                
        public int BrandId { get; set; }
                
        public virtual Brand Brand { get; set; }

        public int OwnerId { get; set; }

        public virtual Employer Owner  { get; set; }

        //public int VignetteId { get; set; }

        //public virtual Vignette Vignette { get; set; }
        public virtual IList<Vignette> Vignettes { get; set; } = new List<Vignette>();

        public virtual IList<VehiclePicture> Pictures { get; set; } = new List<VehiclePicture>();

        public virtual IList<VehicleStuff> Stuff { get; set; } = new List<VehicleStuff>();

        public virtual IList<Rental> Rentals { get; set; } = new List<Rental>();

        public virtual IList<InsurancePolicy> InsurancePolicies { get; set; } = new List<InsurancePolicy>();

        //TODO: apply migration when car status is total damage;
        public virtual IList<VehiclesSpareParts> Parts { get; set; } = new List<VehiclesSpareParts>();
    }
}
