using CarTrade.Data.Models;
using CarTrade.Services.Branches;
using CarTrade.Services.Companies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Web.Areas.Identity.Pages.Account
{
    public class RegisterFormModel : PageModel
    {
        private readonly IBranchesService brancService;
        private readonly ICompaniesService companyService;

        public RegisterFormModel(IBranchesService brancService,
            ICompaniesService companyService)
        {
            this.brancService = brancService;
            var allBranches = this.brancService.AllAsync().Result.AsEnumerable();

            this.Branches = allBranches
                .Select(b => new SelectListItem
                {
                    Text = b.FullAddress,
                    Value = b.Id.ToString()
                })
                .ToList();

            this.companyService = companyService;
            var allCompanies = this.companyService.AllAsync().Result.AsEnumerable();

            this.Companies = allCompanies
               .Select(b => new SelectListItem
               {
                   Text = b.Name,
                   Value = b.Id.ToString()
               })
               .ToList();
        }

        [BindProperty]
        public InputModel Input { get; set; }        

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public List<SelectListItem> Branches { get; }

        public List<SelectListItem> Companies { get; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "User name")]
            public string UserName { get; set; }

            [Required]
            [MinLength(MinNameLength)]
            [MaxLength(MaxNameLength)]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required]
            [MinLength(MinNameLength)]
            [MaxLength(MaxNameLength)]
            [Display(Name = "Second name")]
            public string SecondName { get; set; }

            [Required]
            [MinLength(MinNameLength)]
            [MaxLength(MaxNameLength)]
            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Branch { get; set; }

            [Required]
            public string Employeer { get; set; }
        }


        
    }
}
