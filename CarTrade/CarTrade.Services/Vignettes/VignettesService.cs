using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Services.Vignettes.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using AutoMapper;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Services.Vignettes
{
    //TODO: be consistently  - throw exceptions
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
                throw new ArgumentException(NotExistItemExceptionMessage);
            }
           
            var existVignette = await this.DoesVehicleHaveActiveVignetteAsync(vehicleId);

            if (existVignette)
            {
                throw new ArgumentException(ExistItemExceptionMessage);
            }

            if (vignetteFormModel.StartDate > vignetteFormModel.EndDate)
            {
                throw new ArgumentException(WrongDateExceptionMessage);
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
                throw new ArgumentException(NotExistItemExceptionMessage);
            }

            this.mapper.Map(vignetteFormModel, editVignette);

            await this.db.SaveChangesAsync();
        }

        public async Task<TModel> GetByIdAsync<TModel>(int vignetteId) where TModel : class
        {
            var vignette = await this.db.Vignettes.AnyAsync(v => v.Id == vignetteId);
            if (!vignette)
            {
                throw new ArgumentException(NotExistItemExceptionMessage);
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
                throw new ArgumentException(NotExistItemExceptionMessage);                
            }

            return await this.db.Vignettes
                .Where(v => v.VehicleId == vehicleId)
                .ProjectTo<TModel>()
                .ToListAsync();
        }

        //TODO: if expire logic is passed and Expire is not set true and same day vignette expire 
        public async Task<bool> DoesVehicleHaveActiveVignetteAsync(int vehicleId)
            => await this.db.Vignettes
                .AnyAsync(vg =>
                vg.VehicleId == vehicleId
                && vg.Expired == false
                && vg.EndDate > DateTime.UtcNow);               
               
        public async Task<int> SetVignetteExpireLogicAsync()
        {
            var vignettes = await this.db.Vignettes
               .Where(vg =>
               vg.Expired == false
               && vg.EndDate <= DateTime.UtcNow)
               .ToListAsync();

            foreach (var vignette in vignettes)
            {
                vignette.Expired = true;
            }

            return await this.db.SaveChangesAsync();
        }
    }
}
