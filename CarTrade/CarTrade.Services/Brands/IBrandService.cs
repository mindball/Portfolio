using CarTrade.Services.Brands.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.Brands
{
    public interface IBrandService
    {
        Task AddBrandAsync(string name);

        Task<IEnumerable<BrandListingServiceModel>> AllAsync();

        Task<BrandListingServiceModel> GetByIdAsync(int id);

        Task EditAsync(int id, string name);
    }
}
