using CarTrade.Services.Users.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CarTrade.Web.Models.Users
{
    public class UserListingViewModel
    {
        public IEnumerable<UserListingServiceModel> Users { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
