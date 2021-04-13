using CarTrade.Services.Companies.Models;
using System.Collections.Generic;

namespace CarTrade.Web.Models.Companies
{
    public class CompanyListingViewModel
    {
        public IEnumerable<CompanyListingServiceModel> Companies { get; set; }
    }
}
