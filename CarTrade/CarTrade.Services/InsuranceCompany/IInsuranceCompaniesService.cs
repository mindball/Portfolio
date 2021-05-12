using CarTrade.Services.InsuranceCompany.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Services.InsuranceCompany
{
    public interface IInsuranceCompaniesService
    {
        Task AddInsuranceCompanyAsync(string name);

        Task<IEnumerable<InsuranceCompanyListingServiceModel>> AllAsync();

        Task<TModel> GetByIdAsync<TModel>(int id) where TModel : class;

        Task EditAsync(int id, string name);
    }
}
