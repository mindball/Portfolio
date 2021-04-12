using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Services.Branches.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.Branches
{
    public class BranchesService : IBranchesService
    {
        private readonly CarDbContext db;

        public BranchesService(CarDbContext db)
        {
            this.db = db;
        }
        public async Task<IEnumerable<BrachListingServiceModel>> AllAsync()
            => await this.db.Branches
            .ProjectTo<BrachListingServiceModel>()
            .ToListAsync();
    }
}
