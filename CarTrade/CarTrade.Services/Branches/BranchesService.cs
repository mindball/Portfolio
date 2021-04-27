using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Data.Models;
using CarTrade.Services.Branches.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

        public async Task AddBranchAsync(string town, string address)
        {
            if (town == null && address == null) return;                   

            var newBranch = new Branch
            {
                Town = town,
                Address = address
            };

            await this.db.Branches.AddAsync(newBranch);

            await this.db.SaveChangesAsync();
        }

        public async Task<IEnumerable<BranchListingServiceModel>> AllAsync()
            => await this.db.Branches
            .ProjectTo<BranchListingServiceModel>()
            .ToListAsync();

        public async Task EditAsync(int id, string town, string address)
        {
            var branchToEdit = await this.db.Branches.FirstOrDefaultAsync(b => b.Id == id);

            if (branchToEdit == null || 
                (town == null && address == null))
            {
                return;
            }
           
            branchToEdit.Town = town;
            branchToEdit.Address = address;

            await this.db.SaveChangesAsync();
        }

        public async Task<BranchListingServiceModel> GetByIdAsync(int id)
            => await this.db.Branches
                .Where(b => b.Id == id)
                .ProjectTo<BranchListingServiceModel>()
                .FirstOrDefaultAsync();
    }
}
