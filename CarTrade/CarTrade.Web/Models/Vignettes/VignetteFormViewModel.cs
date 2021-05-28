using CarTrade.Web.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace CarTrade.Web.Models.Vignettes
{
    public class VignetteFormViewModel
    {  
        [Required]
        [DateTimeFromValidateTo(nameof(EndDate))]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int VehicleId { get; set; }

        public bool Expired { get; set; }
    }
}
