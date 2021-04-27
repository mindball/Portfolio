using CarTrade.Services.Users.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.Users
{
    public interface IUsersService
    {
        Task<IEnumerable<UserListingServiceModel>> AllAsync();

        Task<UserListingServiceModel> GetByIdAsync(string userId);
    }
}
