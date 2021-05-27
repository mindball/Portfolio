using AutoMapper;
using CarTrade.Common.Mapping;
using CarTrade.Data.Enums;
using System;

namespace CarTrade.Services.Vehicle.Models
{
    public class VehicleListingServiceModel : VehicleBasicListingServiceModel, IMapFrom<Data.Models.Vehicle>, IHaveCustomMappings
    {   
        public string Description { get; set; }
               
        public DateTime YearОfМanufacture { get; set; }
                      
        public int TravelledDistance { get; set; }
               
        public int EndOilChange { get; set; }

        public DateTime InspectionSafetyCheck { get; set; }

        public VehicleStatus Status { get; set; }

        public int BranchId { get; set; }        

        public string  Branch{ get; set; }

        public int BrandId { get; set; }

        public string BrandName { get; set; }

        public int OwnerId { get; set; }

        public string OwnerName { get; set; }

        //public int InsurancePolicyId { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<Data.Models.Vehicle, VehicleListingServiceModel>()                
                 .ForMember(v => v.OwnerName, cfg => cfg.MapFrom(v => v.Owner.Name))
                 .ForMember(v => v.BrandName, cfg => cfg.MapFrom(v => v.Brand.Name))
                 .ForMember(v => v.Branch, cfg => cfg.MapFrom(v => (v.Branch.Town + " " + v.Branch.Address)))                
                 .ReverseMap()
                 .ForPath(v => v.Owner.Name, opt => opt.Ignore())
                 .ForPath(v => v.Brand.Name, opt => opt.Ignore())
                 .ForPath(v => v.Branch.Town, opt => opt.Ignore());
        }
    }
}
