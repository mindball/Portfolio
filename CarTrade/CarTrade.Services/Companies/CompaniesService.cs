using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Services.Companies.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTrade.Services.Companies
{
    //TODO: be consistently  - throw exceptions
    public class CompaniesService : ICompaniesService
    {
        private readonly CarDbContext db;

        public CompaniesService(CarDbContext db)
        {
            this.db = db;
        }
        public async Task AddCompanyAsync(string name)
        {
            var a = this.db.Companies.Any(b => b.Name == name);

            if (name == null || 
                this.db.Companies.Any(b => b.Name == name)) return;

            await this.db.AddAsync(new Data.Models.Employer { Name = name });
            await this.db.SaveChangesAsync();
        }

        public async Task<IEnumerable<CompanyListingServiceModel>> AllAsync()
        => await this.db.Companies
                .ProjectTo<CompanyListingServiceModel>()
                .ToListAsync();

        public async Task EditAsync(int id, string name)
        {
            var company = await this.db
                .Companies
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null || name == null) return;

            company.Name = name;
            await this.db.SaveChangesAsync();
        }

        public async Task<TModel> GetByIdAsync<TModel>(int insuranceCompanyId) where TModel : class
        {
            var insuranceCompany = await this.db.InsuranceCompanies
               .Where(u => u.Id == insuranceCompanyId)
               .ProjectTo<TModel>()
               .FirstOrDefaultAsync();

            if (insuranceCompany == null)
            {
                throw new ArgumentException($"policy with such {insuranceCompanyId} does not exist");
            }

            return insuranceCompany;
        }

    }
}
