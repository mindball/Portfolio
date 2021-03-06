using CarTrade.Data;
using CarTrade.Data.Models;
using CarTrade.Services;
using CarTrade.Services.Branches;
using CarTrade.Services.Companies;
using CarTrade.Services.Users;
using CarTrade.Services.Users.Models;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    
    public class UsersController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IBranchesService brancService;
        private readonly ICompaniesService employerService;
        

        private readonly CarDbContext dbContext;

        public UsersController(IUsersService usersService,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            CarDbContext dbContext,
            IBranchesService brancService,
            ICompaniesService employerService,
            IMapper mapper)
        {
            this.usersService = usersService;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.brancService = brancService;
            this.employerService = employerService;           

            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var users = await this.usersService.AllAsync();

            return View(new UserListingViewModel
            {
                Users = users
            });
        }

        public async Task<IActionResult> Edit([FromRoute(Name = "id")] string userId)
        {
            var userDetail = await this.usersService.GetByIdAsync<UserEditServiceModel>(userId);
            if (userDetail == null)
            {
                return this.NotFound();
            }

            return this.View(userDetail);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(UserEditServiceModel userModel)
        {
            if (!ModelState.IsValid)
            {
                this.TempData.AddFailureMessage(string.Format(FailureAddItemMessage, userModel.UserName));
                return this.RedirectToAction(nameof(Index));
                //return this.View(userModel);
            }

            var userDetail = await this.usersService.GetByIdAsync<UserEditServiceModel>(userModel.Id);
            if (userDetail == null)
            {
                return this.NotFound();
            }

            await this.usersService.EditUserAsync(userModel.Id, userModel);

            this.TempData.AddSuccessMessage(string.Format(SuccessEditItemMessage, userModel.UserName));
            return RedirectToAction(nameof(Index));
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

        public async Task<IActionResult> AddRole()
        {
            var rolesSelectItems = await this.roleManager
                .Roles
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                })
                .ToListAsync();

            return this.View(rolesSelectItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole([FromRoute(Name = "id")] string userId, string role)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            var roleName = await this.roleManager.FindByNameAsync(role);

            if (user == null || roleName == null)
            {
                this.TempData.AddFailureMessage(string.Format(FailureAddItemMessage, role));
                return this.NotFound();
            }

            var result = await this.userManager.AddToRoleAsync(user, roleName.Name);
            if (!result.Succeeded)
            {
                this.AddModelError(result);
                this.TempData.AddFailureMessage(string.Format(FailureAddItemMessage, role));
                return BadRequest(this.ModelState);
            }

            TempData.AddSuccessMessage($"User {user.UserName} added to {roleName.Name} role!");

            return this.RedirectToAction(nameof(Index));
        }

        //Add custom temp msg to this action ChangePassword, Destroy

        public async Task<IActionResult> ChangePassword([FromRoute(Name = "id")] string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return this.NotFound();
            }

            return this.View(
                new UserChangePasswordViewModel
                {
                    UserName = user.UserName
                });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromRoute(Name = "id")] string userId,
            UserChangePasswordViewModel userModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(userModel);
            }

            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return this.NotFound();
            }
            var token = await this.userManager.GeneratePasswordResetTokenAsync(user);
            var result = await this.userManager.ResetPasswordAsync(user, token, userModel.Password);

            TempData.AddSuccessMessage($"{user.UserName} successfully change password");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete([FromRoute(Name = "id")] string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return this.NotFound();
            }

            return this.View(new UserDetailViewModel
            {
                UserId = user.Id,
                Name = user.FirstName + " " + user.LastName
            });
        }

        public async Task<IActionResult> Destroy([FromRoute(Name = "id")] string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return this.NotFound();
            }

            await this.userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Company([FromRoute(Name = "id")] string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return this.NotFound();
            }           

            return this.View();
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
