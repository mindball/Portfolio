using CarTrade.Services.Vignettes.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.Vignettes
{
    public interface IVignettesService
    {
        Task AddVignetteAsync(int vehicleId, VignetteFormServiceModel vignetteFormModel);

        Task<IEnumerable<VignetteListingServiceModel>> AllAsync();

        Task EditAsync(int vignetteId, VignetteFormServiceModel vignetteFormModel);

        Task<TModel> GetByIdAsync<TModel>(int vignetteId) where TModel : class;

        Task<IEnumerable<TModel>> GetVignetteByVehicleIdAsync<TModel>(int vehicleId) where TModel : class;

        Task<bool> DoesVehicleHaveActiveVignette(int vehicleId);

        Task SetVignetteExpireLogicAsync();
    }
}
