using AutoMapper;
using CarTrade.Common.Mapping;
using CarTrade.Data.Models;

namespace CarTrade.Services.Users.Models
{
    public class UserListingServiceModel : IMapFrom<User>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
               
        public string FirstName { get; set; }
               
        public string SecondName { get; set; }
               
        public string LastName { get; set; }

        public string BranchTown { get; set; }

        public string BranchAddress { get; set; }

        public string EmployerName { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<User, UserListingServiceModel>()
                .ForMember(u => u.BranchTown, cfg => cfg.MapFrom(c => c.Branch.Town))
                .ForMember(u => u.BranchAddress, cfg => cfg.MapFrom(c => c.Branch.Address))
                .ForMember(u => u.EmployerName, cfg => cfg.MapFrom(c => c.Employer.Name));
        }
    }
}
