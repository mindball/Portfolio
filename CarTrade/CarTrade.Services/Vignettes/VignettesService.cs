using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Services.Vignettes.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using AutoMapper;

namespace CarTrade.Services.Vignettes
{
    public class VignettesService : IVignettesService
    {
        private readonly CarDbContext db;
        private readonly IMapper mapper;

        public VignettesService(CarDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        
        public async Task AddVignetteAsync(int vehicleId, VignetteFormServiceModel vignetteFormModel)
        {
            var existVehicle = await this.db.Vehicles.AnyAsync(v => v.Id == vehicleId);            
            if (!existVehicle)
            {
                throw new ArgumentException("this vehicle not exist");
            }

            //TODO: test exist vignette
            var existVignette = await this.db.Vignettes
                .AnyAsync(v => v.VehicleId == vehicleId && !(v.EndDate >= DateTime.UtcNow));
            if(existVignette)
            {
                throw new ArgumentException("vignette has not expire on this vehicle");
            }

            if (vignetteFormModel.StartDate > vignetteFormModel.EndDate)
            {
                throw new ArgumentException("start date must be equal or small than end date");
            }

            var newVignette = this.mapper
                .Map<VignetteFormServiceModel, Data.Models.Vignette>(vignetteFormModel, opt => 
                    opt.ConfigureMap()
                    .ForMember(m => m.Id, opt => opt.Ignore())
                    .ForMember(m => m.Vehicle, opt => opt.Ignore()));

            await this.db.Vignettes.AddAsync(newVignette);
            await this.db.SaveChangesAsync();
        }

        public async Task<IEnumerable<VignetteListingServiceModel>> AllAsync()
            => await this.db.Vignettes
            .OrderBy(v => v.EndDate)
            .ProjectTo<VignetteListingServiceModel>()
            .ToListAsync();

        public async Task EditAsync(int vignetteId, VignetteFormServiceModel vignetteFormModel)
        {
            var editVignette = 
                await this.db.Vignettes.FirstOrDefaultAsync(v => v.Id == vignetteId);
            if(editVignette == null)
            {
                throw new ArgumentException("not exit vignette");
            }

            this.mapper.Map(vignetteFormModel, editVignette);

            await this.db.SaveChangesAsync();
        }

        public async Task<TModel> GetByIdAsync<TModel>(int vignetteId) where TModel : class
        {
            var vignette = await this.db.Vignettes.AnyAsync(v => v.Id == vignetteId);
            if (!vignette)
            {
                throw new ArgumentException("not exist vignette");
            }

            return await this.db.Vignettes
                .Where(v => v.Id == vignetteId)
                .ProjectTo<TModel>()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TModel>> GetVignetteByVehicleIdAsync<TModel>(int vehicleId) where TModel : class
        {
            var vehicle = await this.db.Vignettes.AnyAsync(v => v.VehicleId == vehicleId);
            if (!vehicle)
            {
                throw new ArgumentException("not exist vehicle");
            }

            return await this.db.Vignettes
                .Where(v => v.VehicleId == vehicleId)
                .ProjectTo<TModel>()
                .ToListAsync();
        }
    }
}
