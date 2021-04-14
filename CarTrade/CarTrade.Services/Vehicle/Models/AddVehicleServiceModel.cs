using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Services.Vehicle.Models
{
    public class AddVehicleServiceModel
    {
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
        public string Status { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int OwnerId { get; set; }       
    }
}
