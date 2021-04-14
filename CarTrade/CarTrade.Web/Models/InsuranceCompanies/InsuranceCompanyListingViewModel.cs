using CarTrade.Services.InsuranceCompany.Models;
using System.Collections.Generic;

namespace CarTrade.Web.Models.InsuranceCompanies
{
    public class InsuranceCompanyListingViewModel
    {
        public IEnumerable<InsuranceCompanyListingServiceModel> InsuranceCompanies { get; set; }
    }
}
