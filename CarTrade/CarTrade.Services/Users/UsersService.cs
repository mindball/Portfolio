using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Services.Users.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.Users
{
    class UsersService : IUsersService
    {
        private readonly CarDbContext db;

        public UsersService(CarDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<UserListingServiceModel>> AllAsync()
            => await this.db.Users
            .ProjectTo<UserListingServiceModel>()
            .ToListAsync();
    }
}
