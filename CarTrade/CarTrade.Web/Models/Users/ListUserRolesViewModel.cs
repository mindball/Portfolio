using System.Collections.Generic;

namespace CarTrade.Web.Models.Users
{
    public class ListUserRolesViewModel
    {
        public string UserId { get; set; }

        public string Email { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
