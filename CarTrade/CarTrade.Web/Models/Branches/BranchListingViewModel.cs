using CarTrade.Services.Branches.Models;
using System.Collections.Generic;

namespace CarTrade.Web.Models.Branches
{
    public class BranchListingViewModel
    {
        public IEnumerable<BranchListingServiceModel> Branches { get; set; }        
    }
}
