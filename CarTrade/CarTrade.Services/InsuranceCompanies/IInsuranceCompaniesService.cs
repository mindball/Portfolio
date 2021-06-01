using CarTrade.Services.InsuranceCompanies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.InsuranceCompanies
{
    public interface IInsuranceCompaniesService
    {
        Task AddInsuranceCompanyAsync(string name);

        Task<IEnumerable<InsuranceCompanyListingServiceModel>> AllAsync();

        Task<TModel> GetByIdAsync<TModel>(int id) where TModel : class;

        Task EditAsync(int id, string name);
    }
}
