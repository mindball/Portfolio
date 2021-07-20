using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Services.Brands.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Services.Brands
{    
    public class BrandService : IBrandService
    {
        private readonly CarDbContext db;

        public BrandService(CarDbContext db)
        {
            this.db = db;
        }
        public async Task AddBrandAsync(string name)
        {
            if (this.db.Brands.Any(b => b.Name == name) ||
                string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Name is null or exist brand");
            }

            var newBrand = new Data.Models.Brand { Name = name };

            await this.db.Brands.AddAsync(newBrand);
            await this.db.SaveChangesAsync();
        }

        public async Task<IEnumerable<BrandListingServiceModel>> AllAsync()
            => await this.db.Brands
                .ProjectTo<BrandListingServiceModel>()
                .ToListAsync();

        public async Task EditAsync(int id, string name)
        {
            var brandToEdit = await this.db.Brands.FirstOrDefaultAsync(b => b.Id == id);

            if (brandToEdit == null || string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Name is null or brand is null or not exist");
            }
            
            brandToEdit.Name = name;            

            await this.db.SaveChangesAsync();
        }

        public async Task<BrandListingServiceModel> GetByIdAsync(int id)
            => await this.db.Brands                
                .ProjectTo<BrandListingServiceModel>()
                .FirstOrDefaultAsync(b => b.Id == id);
    }
}
