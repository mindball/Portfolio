using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Services.Users.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

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

        public async Task<UserListingServiceModel> GetByIdAsync(string userId)
        {
            var user = await this.db.Users
                .ProjectTo<UserListingServiceModel>()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new ArgumentException($"user with this id: {userId} doesn't exist");
            }

            return user;
        }
    }
}
