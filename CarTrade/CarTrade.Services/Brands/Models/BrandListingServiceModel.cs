using CarTrade.Common.Mapping;
using System.ComponentModel.DataAnnotations;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Services.Brands.Models
{
    public class BrandListingServiceModel : IMapFrom<CarTrade.Data.Models.Brand>
    {
        public int Id { get; set; }
                
        public string Name { get; set; }
    }
}
