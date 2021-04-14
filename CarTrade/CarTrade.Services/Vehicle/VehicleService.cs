using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Data.Enums;
using CarTrade.Services.Vehicle.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CarTrade.Services.Vehicle
{
    public class VehicleService : IVehicleService
    {
        private readonly CarDbContext db;

        public VehicleService(CarDbContext db)
        {
            this.db = db;
        }
        public async Task AddVehicleAsync(AddVehicleServiceModel vehicleModel)
        {
            var vehicleStatus = VehicleStatus(vehicleModel.Status);

            if (!await this.ValidateVehicleServiceModelAsync(
                vehicleModel.PlateNumber, 
                vehicleModel.Vin, 
                vehicleModel.BranchId, 
                vehicleModel.BrandId, 
                vehicleModel.OwnerId)
                || vehicleStatus == Data.Enums.VehicleStatus.None)
            {
                throw new ArgumentException();
            }

                var newVehicle = this.FillVehicleModel(vehicleModel, vehicleStatus);
                await this.db.Vehicles.AddAsync(newVehicle);
                await this.db.SaveChangesAsync();           
                    
        }

        public async Task EditVehicleAsync(VehicleListingServiceModel vehicleModel)
        {            
            //TODO: implement reverse mapping 

            var vehicle = await this.db.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleModel.Id);

            if (vehicle == null)
            {
                throw new ArgumentException($"No such vehicle id:{vehicleModel.Id}");
            }

            var vehicleStatus = VehicleStatus(vehicleModel.Status.ToString());

            if (!await this.ValidateVehicleServiceModelAsync(
                vehicleModel.PlateNumber,
                vehicleModel.Vin,
                vehicleModel.BranchId,
                vehicleModel.BrandId,
                vehicleModel.OwnerId
                )
                || vehicleStatus == Data.Enums.VehicleStatus.None)
            {
                throw new ArgumentException();
            }

            vehicle.Model = vehicleModel.Model;
            vehicle.PlateNumber = vehicleModel.PlateNumber;
            vehicle.Description = vehicleModel.Description;
            vehicle.YearОfМanufacture = vehicleModel.YearОfМanufacture;
            vehicle.TravelledDistance = vehicleModel.TravelledDistance;
            vehicle.EndOilChange = vehicleModel.EndOilChange;
            vehicle.Vin = vehicleModel.Vin;
            vehicle.Status = vehicleStatus;
            vehicle.BranchId = vehicleModel.BranchId;
            vehicle.BrandId = vehicleModel.BrandId;
            vehicle.OwnerId = vehicleModel.OwnerId;

            await this.db.SaveChangesAsync();
        }

        public async Task<VehicleListingServiceModel> GetByIdAsync(int vehicleId)
        {
            var vehicle = await this.db.Vehicles
                .ProjectTo<VehicleListingServiceModel>()
                .FirstOrDefaultAsync();

            if (vehicle == null)
            {
                throw new ArgumentException($"Vehicle with such {vehicleId} does not exist");
            }

            return vehicle;
        }

        private Data.Models.Vehicle FillVehicleModel(AddVehicleServiceModel vehicleModel, VehicleStatus vehicleStatus)
            => new Data.Models.Vehicle
                {
                    Model = vehicleModel.Model,
                    PlateNumber = vehicleModel.PlateNumber,
                    Description = vehicleModel.Description,
                    YearОfМanufacture = vehicleModel.YearОfМanufacture,
                    TravelledDistance = vehicleModel.TravelledDistance,
                    EndOilChange = vehicleModel.EndOilChange,
                    Vin = vehicleModel.Vin,
                    Status = vehicleStatus,
                    BranchId = vehicleModel.BranchId,
                    BrandId = vehicleModel.BrandId,
                    OwnerId = vehicleModel.OwnerId
                };

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

        private Data.Models.Vehicle FillVehicleModel(string model, string plateNumber,
            string description, DateTime yearОfМanufacture,
            int travelledDistance, int endOilChange,
            string vin, VehicleStatus vehicleStatus,
            int branchId, int brandId, int ownerId)
            => new Data.Models.Vehicle
            {
                Model = model,
                PlateNumber = plateNumber,
                Description = description,
                YearОfМanufacture = yearОfМanufacture,
                TravelledDistance = travelledDistance,
                EndOilChange = endOilChange,
                Vin = vin,
                Status = vehicleStatus,
                BranchId = branchId,
                BrandId = brandId,
                OwnerId = ownerId
            };

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
