using System.ComponentModel.DataAnnotations;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Web.Models.Users
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(MinNameLength)]
        [MaxLength(MaxNameLength)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(MinNameLength)]
        [MaxLength(MaxNameLength)]
        public string SecondName { get; set; }

        public string LastName { get; set; }

        [Required]
        public string BranchTown { get; set; }

        [Required]
        public string BranchAddress { get; set; }

        [Required]
        public string CompanyName { get; set; }
    }
}
