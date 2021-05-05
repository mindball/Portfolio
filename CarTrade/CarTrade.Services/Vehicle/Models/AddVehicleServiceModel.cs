using CarTrade.Common.Mapping;
using CarTrade.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Services.Vehicle.Models
{
    public class AddVehicleServiceModel : IMapTo<Data.Models.Vehicle>
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
        public VehicleStatus Status { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int OwnerId { get; set; }       
    }
}
