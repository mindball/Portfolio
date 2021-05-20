using CarTrade.Services.Branches.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.Branches
{
    public interface IBranchesService
    {
        Task<IEnumerable<BranchListingServiceModel>> AllAsync();

        Task AddBranchAsync(string town, string address);

        Task<BranchListingServiceModel> GetByIdAsync(int id);

        Task EditAsync(int id, string town, string address);        

        //TODO: Check if Branch exist by name adress.
    }
}
