using CarTrade.Services.Branches.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.Branches
{
    public interface IBranchesService
    {
        Task<IEnumerable<BranchListingServiceModel>> AllAsync();

        Task AddBranchAsync(string town, string address);

        Task<TModel> GetByIdAsync<TModel>(int id);

        Task EditAsync(int id, string town, string address);

        //TODO: Check if Branch exist by name adress.
        Task<BranchVehiclesListingServiceModel> GetAllVehicleByBranchAsync(int id);

        Task<BranchVehiclesListingServiceModel> GetAllVehicleByGivenUserBranchAddress(string userId);

        Task<bool> IsThisUserEmployeeInThisBranch(int branchId, string userId);
    }
}
