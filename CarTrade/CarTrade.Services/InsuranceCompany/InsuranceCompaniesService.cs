using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Services.InsuranceCompany.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTrade.Services.InsuranceCompany
{
    public class InsuranceCompaniesService : IInsuranceCompaniesService
    {
        private readonly CarDbContext db;

        public InsuranceCompaniesService(CarDbContext db)
        {
            this.db = db;
        }

        public async Task AddInsuranceCompanyAsync(string name)
        {
            if (name == null
                || this.db.InsuranceCompanies
                .Any(i => i.Name == name)) return;

            var newInsuranceCompany = new Data.Models.InsuranceCompany { Name = name };
            await this.db.InsuranceCompanies.AddAsync(newInsuranceCompany);
            await this.db.SaveChangesAsync();
        }

        public async Task<IEnumerable<InsuranceCompanyListingServiceModel>> AllAsync()
                => await this.db.InsuranceCompanies
            .ProjectTo<InsuranceCompanyListingServiceModel>()
            .ToListAsync();

        public async Task EditAsync(int id, string name)
        {
            var insuranceCompany = await this.db
                .InsuranceCompanies
                .FirstOrDefaultAsync(c => c.Id == id);

            if (insuranceCompany == null ||
                name == null) return;

            insuranceCompany.Name = name;
            await this.db.SaveChangesAsync();
        }

        public async Task<InsuranceCompanyListingServiceModel> GetByIdAsync(int id)
         => await this.db.InsuranceCompanies
                .ProjectTo<InsuranceCompanyListingServiceModel>()
                .FirstOrDefaultAsync(b => b.Id == id);
    }
}
