using CarTrade.Common.Mapping;
using CarTrade.Data.Models;

namespace CarTrade.Services.Branches.Models
{
    public class BranchListingServiceModel : IMapFrom<Branch>
    {
        public int Id { get; set; }
        
        public string Town { get; set; }

        public string Address { get; set; }

        public string FullAddress 
        {
            get => $"{this.Town} {this.Address}";             
        }
    }
}
