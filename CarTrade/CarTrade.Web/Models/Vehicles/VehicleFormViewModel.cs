using CarTrade.Common.Mapping;
using CarTrade.Data.Enums;
using CarTrade.Services.Vehicle.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Web.Models.Vehicles
{
    public class VehicleFormViewModel : IValidatableObject
    {
        [Required]
        [MinLength(VehicleModelTypeMinLength)]
        [MaxLength(VehicleModelTypeMaxLength)]
        public string Model { get; set; }

        [Required]
        [MinLength(PlateNumberMinLength)]
        [MaxLength(PlateNumberМaxLength)]
        [Display(Name = "Plate Number")]
        public string PlateNumber { get; set; }

        [MaxLength(DescriptionLength)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Year of Manufacture")]
        [DataType(DataType.Date)]
        public DateTime YearОfМanufacture { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Travelled Distance")]
        public int TravelledDistance { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Oil change date")]
        public int EndOilChange { get; set; }

        [Required]
        [MinLength(VinMinLength)]
        [MaxLength(VinMaxLength)]
        public string Vin { get; set; }

        [Required]
        [Display(Name = "Inspection safety check")]
        [DataType(DataType.Date)]
        public DateTime InspectionSafetyCheck { get; set; }

        [Required]
        [Display(Name = "Status")]
        public VehicleStatus Status { get; set; }

        [Required]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }       

        [Required]
        [Display(Name = "Brand")]
        public int BrandId { get; set; }       

        [Required]
        [Display(Name = "Yemployer")]
        public int OwnerId { get; set; }

        public CollectCompanyDetailsViewModel CollectCompanyDetails { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.TravelledDistance >= this.EndOilChange)
            {
                yield return new ValidationResult("The distance traveled should be less than before changing the oil");
            }
        }
    }
}
