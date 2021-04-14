using CarTrade.Services.Vehicle.Models;
using System;
using System.Threading.Tasks;

namespace CarTrade.Services.Vehicle
{
    public interface IVehicleService
    {
        Task AddVehicleAsync(AddVehicleServiceModel vehicleModel);

        Task EditVehicleAsync(VehicleListingServiceModel vehicleModel);

        Task<VehicleListingServiceModel> GetByIdAsync(int insuranceId);
    }
}
