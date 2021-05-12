using CarTrade.Services.Vehicle.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.Vehicle
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleListingServiceModel>> AllAsync();

        Task AddVehicleAsync(AddVehicleServiceModel vehicleModel);

        Task EditVehicleAsync(int vehicleId, AddVehicleServiceModel vehicleModel);

        Task<TModel> GetByIdAsync<TModel>(int vehicleId) where TModel : class;
    }
}
