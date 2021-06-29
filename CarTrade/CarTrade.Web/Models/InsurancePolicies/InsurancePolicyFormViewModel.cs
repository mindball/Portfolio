using CarTrade.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarTrade.Web.Models.InsurancePolicies
{
    public class InsurancePolicyFormViewModel : IValidatableObject
    {
        [Required]
        [Display(Name = "Type of insurance")]
        public TypeInsurance TypeInsurance { get; set; }

        [Required]
        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        //[DateTimeFromValidateTo(nameof(EndDate))]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End date")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime EndDate { get; set; }
                
        public bool? Expired { get; set; }              

        [Display(Name = "Insurance company")]
        public string InsuranceCompanyId { get; set; }

        public IEnumerable<SelectListItem> InsuranceCompanies { get; set; }        

        //[Required]
        //public int VehicleId { get; set; }

        //public virtual Vehicle Vehicle { get; set; }

        //Validation in viewModel
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.StartDate <= DateTime.UtcNow.AddYears(-1))
            {
                yield return new ValidationResult("Start date must not be less than one year.");
            }

            if (this.StartDate > this.EndDate)
            {
                yield return new ValidationResult("Start date should be before end date.");
            }
        }
    }
}
