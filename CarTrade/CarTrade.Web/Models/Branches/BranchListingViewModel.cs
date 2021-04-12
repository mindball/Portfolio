using CarTrade.Services.Branches.Models;
using System.Collections.Generic;

namespace CarTrade.Web.Models.Branches
{
    public class BranchListingViewModel
    {
        public IEnumerable<BrachListingServiceModel> Branches { get; set; }        
    }
}
