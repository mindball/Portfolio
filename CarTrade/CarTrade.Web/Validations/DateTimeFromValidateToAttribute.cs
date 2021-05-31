using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Web.Validations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateTimeFromValidateToAttribute : ValidationAttribute
    {
        public DateTimeFromValidateToAttribute(string dateToCompareToFieldName)
        {
            this.EndDate = dateToCompareToFieldName;
        }

        private string EndDate { get; set; }

        protected override ValidationResult IsValid(object startDate, ValidationContext validationContext)
        {
            DateTime earlierDate = (DateTime)startDate;

            DateTime laterDate = (DateTime)validationContext.ObjectType.GetProperty(this.EndDate).GetValue(validationContext.ObjectInstance, null);
            
            if (laterDate > earlierDate)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Later date  is equal or less than Start date");
        }
    }
}
