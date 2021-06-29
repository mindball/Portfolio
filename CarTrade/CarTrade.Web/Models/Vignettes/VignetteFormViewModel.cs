using CarTrade.Web.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace CarTrade.Web.Models.Vignettes
{
    public class VignetteFormViewModel
    {  
        [Required]
        [DateTimeFromValidateTo(nameof(EndDate))]
        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End date")]
        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTime EndDate { get; set; }

        [Required]
        public int VehicleId { get; set; }

        public bool Expired { get; set; }
    }
}
