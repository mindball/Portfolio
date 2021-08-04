using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Data.Models;
using CarTrade.Services.Users.Models;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> userManager;

        public UsersService(CarDbContext db, IMapper mapper, UserManager<User> userManager)
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

        public async Task<List<TModel>> GetUsersByBranchAsync<TModel>(int branchId)
             where TModel : class
        {           
            var users = await this.db.Users
                .Where(u => u.BranchId == branchId)
                .ProjectTo<TModel>()
                .ToListAsync();

            //TODO: every branch has a manage and users
            //if (users == null)
            //{               
            //    throw new ArgumentException($"users with this branchId: {branchId} doesn't exist");               
            //}

            return users;
        }

        //TODO: make async method to be a consistency
        public List<UserWithRoleIdServiceModel> GetUsersByRole(int branchId, string roleId)
        {
            var result = from usr in db.Users
                              where usr.BranchId == branchId
                              join ur in db.UserRoles on usr.Id equals ur.UserId
                              where ur.RoleId == roleId                              
                              select new 
                              {
                                  usr.Id,
                                  usr.UserName,
                                  usr.Email,
                                  ur.RoleId,
                                  usr.Branch.Town,
                                  usr.Branch.Address
                              };            

           var usersByRoles = new List<UserWithRoleIdServiceModel>();

            foreach (var userByRole in   result)
            {
                usersByRoles.Add(new UserWithRoleIdServiceModel
                {
                    Id = userByRole.Id,
                    Username = userByRole.UserName,
                    Email = userByRole.Email,
                    RoleId = userByRole.RoleId,
                    Town = userByRole.Town,
                    Address = userByRole.Address
                });
            }

           return usersByRoles;

            //TODO: it took me too long and didn't work
            //var usersByRole = await this.db.Users
            //    .Where(u => u.BranchId == branchId)
            //    .Join(this.db.UserRoles,
            //       u => u.Id,
            //       ur => ur.RoleId,
            //       (u, ur) =>  new
            //       {
            //           u.UserName,
            //           u.Email,
            //           ur.RoleId
            //       }
            //    ).Where(r => r.RoleId == roleId)
            //    .Select(a => new UserWithRoleIdServiceModel
            //    {
            //        Username = a.UserName,
            //        Email = a.Email,
            //        RoleId = a.RoleId
            //    })
            //    .ToListAsync();

            //return usersByRole;
        }
    }
}
