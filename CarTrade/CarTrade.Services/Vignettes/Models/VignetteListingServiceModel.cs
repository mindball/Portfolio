using AutoMapper;
using CarTrade.Common.Mapping;
using System;

namespace CarTrade.Services.Vignettes.Models
{
    public class VignetteListingServiceModel : IMapFrom<Data.Models.Vignette>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public int VehicleId { get; set; }

        public string PlateNumber { get; set; }

        public string Vin { get; set; }        
       
        public DateTime StartDate { get; set; }
       
        public DateTime EndDate { get; set; }

        public bool Expired { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<Data.Models.Vignette, VignetteListingServiceModel>()
                .ForMember(vg => vg.PlateNumber, cfg => cfg.MapFrom(v => v.Vehicle.PlateNumber))
                .ForMember(vg => vg.Vin, cfg => cfg.MapFrom(v => v.Vehicle.Vin));
        }
    }
}
