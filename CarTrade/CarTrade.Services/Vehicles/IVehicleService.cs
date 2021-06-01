using CarTrade.Services.Vehicles.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.Vehicles
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleListingServiceModel>> AllAsync();

        Task AddVehicleAsync(AddVehicleServiceModel vehicleModel);

        Task EditVehicleAsync(int vehicleId, AddVehicleServiceModel vehicleModel);

        Task<IEnumerable<VehicleListExpireInsurancePolicyServiceModel>> GetInsuranceExpireDataAsync(int branchId);

        Task<IEnumerable<VehicleListExpireVignetteServiceModel>> GetVignetteExpireDataAsync(int branchId);

        Task<IEnumerable<VehicleListingChangeOilServiceModel>> GetOilChangeExpireDataAsync(int branchId);

        Task<IEnumerable<VehicleListingInspectionSafetyCheckServiceModel>> GetInspectionSafetyCheckExpireDataAsync(int branchId);

        Task<TModel> GetByIdAsync<TModel>(int vehicleId) where TModel : class;       
    }
}
