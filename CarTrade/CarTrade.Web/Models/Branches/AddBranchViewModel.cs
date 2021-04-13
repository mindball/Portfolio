using System.ComponentModel.DataAnnotations;

namespace CarTrade.Web.Models.Branches
{
    public class AddBranchViewModel
    {
        [Required]
        public string Town { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
