using CarTrade.Data;
using CarTrade.Data.Models;
using CarTrade.Services.Users;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    [Authorize(Roles = AdministratorRole)]
    public class UsersController : Controller
    {
        private readonly IUsersService usersService;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly CarDbContext dbContext;

        public UsersController(IUsersService usersService,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            CarDbContext dbContext)
        {
            this.usersService = usersService;
            this.userManager = userManager;
            this.roleManager = roleManager;

            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var users = await this.usersService.AllAsync();
            //var userIds = this.dbContext.Users.ToList();

            //this.userManager.

            //var roles = await this.roleManager
            //.Roles
            //.Select(r => new SelectListItem
            //{
            //    Text = r.Name,
            //    Value = r.Name
            //})
            //.ToListAsync();

            return View(new UserListingViewModel
            {
                Users = users
            });
        }

        public async Task<IActionResult> UserRoles(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);
            if (user == null)
            {
                return this.NotFound();
            }

            var userRoles = await this.userManager.GetRolesAsync(user);
            var viewModelResults = new ListUserRolesViewModel()
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = userRoles
            };

            return this.View(viewModelResults);
        }

        public async Task<IActionResult> AddRole(string userId)
        {
            var rolesSelectItems = this.roleManager
                .Roles
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                })
                .ToList();

            return this.View(rolesSelectItems);
        }

        [HttpPost]        
        public async Task<IActionResult> AddRole(string id, string role)
        {
            var user = await this.userManager.FindByIdAsync(id);
            var roleName = await this.roleManager.FindByNameAsync(role);

            if (user == null || roleName == null)
            {
                return this.NotFound();
            }

            var result = await this.userManager.AddToRoleAsync(user, roleName.Name);
            if (!result.Succeeded)
            {
                this.AddModelError(result);
                return BadRequest(this.ModelState);
            }

            //TempDataPopUpMessage.AddSuccessMessage(GlobalConstants.SuccessMsg,
            //         $"User {user.UserName} added to {roleName.Name} role!");

            return this.RedirectToAction(nameof(Index));
        }

        private void AddModelError(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
