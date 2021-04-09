using System.ComponentModel.DataAnnotations;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Data.Models
{
    public class VehiclePicture
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(PicUrlLength)]
        public string Url { get; set; }

        public int VehicleId { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
