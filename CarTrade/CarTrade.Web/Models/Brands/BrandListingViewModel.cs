using CarTrade.Services.Brands.Models;
using System.Collections.Generic;

namespace CarTrade.Web.Models.Brands
{
    public class BrandListingViewModel
    {
        public IEnumerable<BrandListingServiceModel> Brands { get; set; }
    }
}
