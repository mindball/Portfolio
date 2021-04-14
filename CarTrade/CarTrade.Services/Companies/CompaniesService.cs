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
    public class CompaniesService : ICompaniesService
    {
        private readonly CarDbContext db;

        public CompaniesService(CarDbContext db)
        {
            this.db = db;
        }
        public async Task AddCompanyAsync(string name)
        {
            if (name == null || !this.db.Companies.Any(b => b.Name == name)) return;

            await this.db.AddAsync(new Data.Models.Company { Name = name });
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
                .FirstOrDefaultAsync(c => c.Id == id && c.Name == name);

            if (company == null || name == null) return;

            company.Name = name;
            await this.db.SaveChangesAsync();
        }

        public async Task<CompanyListingServiceModel> GetByIdAsync(int id)
            => await this.db
               .Companies
               .ProjectTo<CompanyListingServiceModel>()
               .FirstOrDefaultAsync(c => c.Id == id);
        
    }
}
