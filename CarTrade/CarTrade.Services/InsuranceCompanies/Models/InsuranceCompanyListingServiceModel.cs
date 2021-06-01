using CarTrade.Common.Mapping;

namespace CarTrade.Services.InsuranceCompanies.Models
{
    public class InsuranceCompanyListingServiceModel : IMapFrom<Data.Models.InsuranceCompany>
    {
        public int Id { get; set; }

        public string Name { get; set; }                
    }
}
