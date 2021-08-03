using CarTrade.Services.Users.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.Users
{
    public interface IUsersService
    {
        Task<IEnumerable<UserListingServiceModel>> AllAsync();

        Task<TModel> GetByIdAsync<TModel>(string userId) where TModel : class;

        //TODO: reverse map
        Task EditUserAsync(string userId, UserEditServiceModel model);

        Task<List<TModel>> GetUsersByBranchAsync<TModel>(int branchId) where TModel : class;

        List<UserWithRoleIdServiceModel> GetUsersByRole(int branchId, string roleId);
    }
}
