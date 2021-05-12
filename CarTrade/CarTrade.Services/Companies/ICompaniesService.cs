using CarTrade.Services.Companies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.Companies
{
    public interface ICompaniesService
    {
        Task AddCompanyAsync(string name);

        Task<IEnumerable<CompanyListingServiceModel>> AllAsync();

        Task<TModel> GetByIdAsync<TModel>(int insuranceCompanyId) where TModel : class;

        Task EditAsync(int id, string name);
    }
}
