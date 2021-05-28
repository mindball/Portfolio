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

using static CarTrade.Common.DataConstants;

namespace CarTrade.Services.Vehicle
{
    //TODO: be consistently  - throw exceptions
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

        public async Task<IEnumerable<VehicleListExpireInsurancePolicyServiceModel>> GetInsuranceExpireDataAsync(int branchId)
            => await this.db.Vehicles
                 .Where(b => b.BranchId == branchId &&
                        b.InsurancePolicies
                        .Any(i => i.EndDate <= DateTime.UtcNow.AddDays(DaysBeforeItExpires)))
                 .Select(v => new VehicleListExpireInsurancePolicyServiceModel
                 {                     
                     VehicleId = v.Id,
                     PlateNumber = v.PlateNumber,
                     Vin = v.Vin,                     
                     InsurancePolicies = v.InsurancePolicies
                                .Where(i => i.EndDate <= DateTime.UtcNow.AddDays(DaysBeforeItExpires))                                
                                .Select(i => i)
                                .ToList()
                 })
                 .ToListAsync();

        public async Task<IEnumerable<VehicleListExpireVignetteServiceModel>> GetVignetteExpireDataAsync(int branchId)
        => await this.db.Vehicles
                 .Where(v => v.BranchId == branchId
                    && v.Vignettes.Any(vg => vg.VehicleId == v.Id && vg.EndDate <= DateTime.UtcNow.AddDays(DaysBeforeItExpires)))                        
                 .Select(v => new VehicleListExpireVignetteServiceModel
                 {
                     VehicleId = v.Id,
                     PlateNumber = v.PlateNumber,
                     Vin = v.Vin,
                     ExpireDate = v.Vignettes.Where(vh => vh.VehicleId == v.Id).Select(vg => vg.EndDate).FirstOrDefault()
                 })
                 .ToListAsync();

        public async Task<IEnumerable<VehicleListingChangeOilServiceModel>> GetOilChangeExpireDataAsync(int branchId)
            => await this.db.Vehicles
                .Where(v => v.BranchId == branchId
                        && (v.TravelledDistance + RemainDistanceOilChange) >= v.EndOilChange)
                .Select(v => new VehicleListingChangeOilServiceModel
                {                    
                    VehicleId = v.Id,
                    PlateNumber = v.PlateNumber,
                    Vin = v.Vin,
                    EndOilChange = v.EndOilChange
                })
                .ToListAsync();

        public async Task<IEnumerable<VehicleListingInspectionSafetyCheckServiceModel>> GetInspectionSafetyCheckExpireDataAsync(int branchId)
        => await this.db.Vehicles
            .Where(v => v.BranchId == branchId
                    && (v.InspectionSafetyCheck <= DateTime.UtcNow.AddDays(DaysBeforeItExpires)))
            .Select(v => new VehicleListingInspectionSafetyCheckServiceModel
            {
                VehicleId = v.Id,
                PlateNumber = v.PlateNumber,
                Vin = v.Vin,
                InspectionSafetyCheck = v.InspectionSafetyCheck
            })
            .ToListAsync();

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
