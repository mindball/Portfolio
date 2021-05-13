using CarTrade.Common.Mapping;
using CarTrade.Data.Enums;
using CarTrade.Services.InsurancePolicy.Models;
using System;

namespace CarTrade.Web.Models.InsurancePolicy
{
    public class DetailInsurancePolicyViewModel : IMapFrom<InsurancePolicyListingServiceModel>
    {
        public int Id { get; set; }        

        public TypeInsurance TypeInsurance { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Expired { get; set; }

        public int InsuranceCompanyId { get; set; }

        //public string InsuranceCompanyName { get; set; }               
    }
}
