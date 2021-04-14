using CarTrade.Common.Mapping;

namespace CarTrade.Services.InsuranceCompany.Models
{
    public class InsuranceCompanyListingServiceModel : IMapFrom<Data.Models.InsuranceCompany>
    {
        public int Id { get; set; }

        public string Name { get; set; }                
    }
}
