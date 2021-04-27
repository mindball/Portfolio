using System.ComponentModel.DataAnnotations;

namespace CarTrade.Web.Models
{
    public abstract class BasicDetailViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
