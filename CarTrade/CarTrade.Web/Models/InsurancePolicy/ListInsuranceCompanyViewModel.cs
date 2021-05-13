using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CarTrade.Web.Models.InsurancePolicy
{
    public class ListInsuranceCompanyViewModel
    {
        public InsurancePolicyFormViewModel InsurancePolicy { get; set; }

        public IEnumerable<SelectListItem> InsuranceCompanies { get; set; }
    }
}
