using CarTrade.Services.Branches.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.Branches
{
    public interface IBranchesService
    {
        Task<IEnumerable<BrachListingServiceModel>> AllAsync();

        Task AddBranchAsync(string town, string address);
    }
}
