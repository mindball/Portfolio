using CarTrade.Services.InsuranceCompanies.Models;
using CarTrade.Services.InsurancePolicies.Models;
using System.Collections.Generic;

namespace CarTrade.Web.Models.InsuranceCompanies
{
    public class InsuranceCompanyListingViewModel
    {
        public IEnumerable<InsuranceCompanyListingServiceModel> InsuranceCompanies { get; set; }
    }
}
