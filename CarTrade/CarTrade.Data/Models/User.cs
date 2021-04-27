using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Data.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MinLength(MinNameLength)]
        [MaxLength(MaxNameLength)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(MinNameLength)]
        [MaxLength(MaxNameLength)]
        public string SecondName { get; set; }

        [Required]
        [MinLength(MinNameLength)]
        [MaxLength(MaxNameLength)]
        public string LastName { get; set; }

        public int EmployerId { get; set; }

        public virtual Employer Employer { get; set; }

        public int BranchId { get; set; }

        public virtual Branch Branch { get; set; }        

        public virtual IList<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
