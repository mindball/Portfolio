using CarTrade.Common.Mapping;

namespace CarTrade.Services.Companies.Models
{
    public class CompanyListingServiceModel : IMapFrom<Data.Models.Company>
    {
        public int Id { get; set; }
                
        public string Name { get; set; }
    }
}
