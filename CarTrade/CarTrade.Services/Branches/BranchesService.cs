using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Data.Models;
using CarTrade.Services.Branches.Models;
using CarTrade.Services.Vehicles.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace CarTrade.Services.Branches
{
    //TODO: be consistently  - throw exceptions
    public class BranchesService : IBranchesService
    {
        private readonly CarDbContext db;

        public BranchesService(CarDbContext db)
        {
            this.db = db;
        }

        public async Task AddBranchAsync(string town, string address)
        {
            if (town == null && address == null)
            {
                throw new ArgumentNullException("Neither the city nor the address should be null");
            }

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
                throw new ArgumentNullException("Neither the city nor the address should be null");
            }

            branchToEdit.Town = town;
            branchToEdit.Address = address;

            await this.db.SaveChangesAsync();
        }

        public async Task<TModel> GetByIdAsync<TModel>(int id)
            => await this.db.Branches
                .Where(b => b.Id == id)
                .ProjectTo<TModel>()
                .FirstOrDefaultAsync();

        //TODO: test
        public async Task<BranchVehiclesListingServiceModel> GetAllVehicleByBranchAsync(int id)
            => await this.db.Branches.Where(b => b.Id == id)
                        .Select(b => new BranchVehiclesListingServiceModel
                        {
                            FullAddress = string.Join(' ', b.Town, b.Address),
                            AllVehicles = b.Vehicles
                                    .Where(v => v.BranchId == b.Id)
                                    .Select(v => new VehicleBasicListingServiceModel
                                    {
                                        Id = v.Id,
                                        Model = v.Model,
                                        PlateNumber = v.PlateNumber,
                                        Vin = v.Vin
                                    }).ToList()
                        }).FirstOrDefaultAsync();

        public async Task<BranchVehiclesListingServiceModel> GetAllVehicleByGivenUserBranchAddress(string userId)
        {
            var branchId = await this.db.Users
                .Where(u => u.Id == userId)
                .Select(u => u.BranchId)
                .FirstOrDefaultAsync();

            return await this.GetAllVehicleByBranchAsync(branchId);
        }

        public async Task<bool> IsThisUserEmployeeInThisBranch(int branchId, string userId)
            => await this.db.Users.AnyAsync(u => u.Id == userId && u.BranchId == branchId);
    }
}
