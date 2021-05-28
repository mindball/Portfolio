using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CarTrade.Services.Vignettes.Models
{
    public class VignetteFormServiceModel
    { 
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int VehicleId { get; set; }

        public bool Expired { get; set; }
    }
}
