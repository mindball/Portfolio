using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Data.Enums;
using CarTrade.Services.Vehicle.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Services.Vehicle
{
    public class VehicleService : IVehicleService
    {
        private readonly CarDbContext db;
        private readonly IMapper mapper;

        public VehicleService(CarDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<VehicleListingServiceModel>> AllAsync()
            => await this.db.Vehicles
            .ProjectTo<VehicleListingServiceModel>()
            .ToListAsync();

        public async Task AddVehicleAsync(AddVehicleServiceModel vehicleModel)
        {
            //var vehicleStatus = VehicleStatus(vehicleModel.Status);

            if (!await this.ValidateVehicleServiceModelAsync(
                vehicleModel.PlateNumber, 
                vehicleModel.Vin, 
                vehicleModel.BranchId, 
                vehicleModel.BrandId, 
                vehicleModel.OwnerId)
                || vehicleModel.Status == Data.Enums.VehicleStatus.None)
            {
                throw new ArgumentException();
            }

            //var newVehicle = this.FillVehicleModel(vehicleModel, vehicleStatus);
            var newVehicle = this.mapper.Map<Data.Models.Vehicle>(vehicleModel);

                await this.db.Vehicles.AddAsync(newVehicle);
                await this.db.SaveChangesAsync();           
                    
        }

        public async Task EditVehicleAsync(int vehicleId, AddVehicleServiceModel vehicleModel)
        {
            var vehicle = await this.db.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId);
            //TODO: Catch custom exception
            if (vehicle == null)
            {
                throw new ArgumentException($"No such vehicle id:{vehicleId}");
            }

            this.mapper.Map(vehicleModel, vehicle);           

            await this.db.SaveChangesAsync();
        }

        public async Task<TModel> GetByIdAsync<TModel>(int vehicleId)
            where TModel : class
        {
            var vehicle = await this.db.Vehicles
                .Where(u => u.Id == vehicleId)
                .ProjectTo<TModel>()
                .FirstOrDefaultAsync();

            if (vehicle == null)
            {
                throw new ArgumentException($"Vehicle with such {vehicleId} does not exist");
            }

            return vehicle;
        }        

        private async Task<bool> ValidateVehicleServiceModelAsync(string plateNumber, 
            string vin, 
            int branchId, 
            int brandId, 
            int ownerId)
        {
            if (await this.db.Vehicles
               .AnyAsync(v => v.PlateNumber == plateNumber))
            {
                throw new ArgumentException($"{plateNumber} exist");
            }

            if (await this.db.Vehicles
                .AnyAsync(v => v.Vin == vin))
            {
                throw new ArgumentException($"{vin} exist");
            }

            var existBranchId = await this.db.Branches
                .AnyAsync(b => b.Id == branchId);

            if (!existBranchId)
            {
                throw new ArgumentException($"Branch id {branchId} not exist");
            }

            var existBrandId = await this.db.Brands
                .AnyAsync(b => b.Id == brandId);
            if (!existBrandId)
            {
                throw new ArgumentException($"Brand: id {brandId} not exist");
            }

            var existOwnerId = await this.db.Companies
                .AnyAsync(b => b.Id == ownerId);
            if (!existOwnerId)
            {
                throw new ArgumentException($"Company: id {ownerId} not exist");
            }

            return true;
        }

        private VehicleStatus VehicleStatus(string status)
        {
            VehicleStatus vehicleStatus;
            if (!Enum.TryParse(status, out vehicleStatus))
            {
                vehicleStatus = Data.Enums.VehicleStatus.None;
            }

            return vehicleStatus;
        }       
    }
}
