using CarTrade.Services.Vignettes.Models;
using System.Collections.Generic;

namespace CarTrade.Web.Models.Vignettes
{
    public class VignetteListingViewModel
    {
        public IEnumerable<VignetteListingServiceModel> AllVignettes { get; set; }
    }
}
