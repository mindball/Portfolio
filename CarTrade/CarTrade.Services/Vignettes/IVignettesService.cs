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
    }
}
