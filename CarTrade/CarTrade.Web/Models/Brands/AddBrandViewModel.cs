using System.ComponentModel.DataAnnotations;

namespace CarTrade.Web.Models.Brands
{
    public class AddBrandViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
