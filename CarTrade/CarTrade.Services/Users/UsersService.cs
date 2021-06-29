using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Services.Users.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Services.Users
{
    //TODO: be consistently  - throw exceptions
    public class UsersService : IUsersService
    {
        private readonly CarDbContext db;
        private readonly IMapper mapper;

        public UsersService(CarDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<UserListingServiceModel>> AllAsync()
            => await this.db.Users
                    .ProjectTo<UserListingServiceModel>()
                    .ToListAsync();

        public async Task EditUserAsync(string userId, UserEditServiceModel model)
        {
            var user = await this.db.Users.FindAsync(userId);
            if (user == null) throw new ArgumentException("user doesn't exist");

            this.mapper.Map(model, user);

            this.db.Update(user);
            await this.db.SaveChangesAsync();                        
        }        

        public async Task<TModel> GetByIdAsync<TModel>(string userId) 
            where TModel : class
        {
            var user = await this.db.Users
                .Where(u => u.Id == userId)
                .ProjectTo<TModel>()
                .FirstOrDefaultAsync();                

            if (user == null)
            {
                throw new ArgumentException($"user with this id: {userId} doesn't exist");
            }

            return user;
        }
    }
}
