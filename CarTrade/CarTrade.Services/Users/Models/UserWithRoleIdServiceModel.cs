using System;
using System.Diagnostics.CodeAnalysis;

namespace CarTrade.Services.Users.Models
{
    public class UserWithRoleIdServiceModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string RoleId { get; set; }

        public string Town { get; set; }

        public string Address { get; set; }

        public string FullAddress 
        { 
            get
            {
                return this.Town + " " + this.Address;
            }
           
        }
    }
}
