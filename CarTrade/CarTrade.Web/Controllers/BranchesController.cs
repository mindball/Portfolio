using CarTrade.Data.Models;
using CarTrade.Services.Branches;
using CarTrade.Services.Branches.Models;
using CarTrade.Services.Users;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Models.Branches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

using CarTrade.Common;
using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    public class BranchesController : ManagerController
    {
        private readonly IBranchesService branchesService;
        private readonly UserManager<User> userManager;

        public BranchesController(IBranchesService branchesService,
            UserManager<User> userManager)
        {
            this.branchesService = branchesService;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var branches = await this.branchesService.AllAsync();

            return View(new BranchListingViewModel
            {
                Branches = branches
            });
        }
                
        [AllowAnonymous]
        public async Task<IActionResult> ListVehicles([FromRoute(Name = "id")] int branchId)
        {
            if(this.User.IsInRole(DataConstants.AdministratorRole))
            {
                var branch = await this.branchesService.GetByIdAsync<BranchVehiclesListingServiceModel>(branchId);
                if (branch == null)
                {
                    return this.BadRequest();
                }

                var allVehicle = await this.branchesService.GetAllVehicleByBranchAsync(branchId);

                return this.View(allVehicle);
            }

            if(this.User.IsInRole(DataConstants.ManagerRole))
            {
                //var branch = await this.branchesService.GetByIdAsync<BranchVehiclesListingServiceModel>(branchId);
                //if (branch == null)
                //{
                //    return this.BadRequest();
                //}
                var userId = (await this.userManager.GetUserAsync(this.User)).Id;
                bool existUserInCurrentBranch =
                    await this.branchesService.IsThisUserEmployeeInThisBranch(branchId, userId);
                if (!existUserInCurrentBranch)
                {
                    this.TempData[AccessDenied] = AccessDenied;
                    return this.RedirectToAction("Index", "Home");
                }                
                
                var listAllVehicleFilterByUserBranchAddress = 
                    await this.branchesService.GetAllVehicleByGivenUserBranchAddress(userId);

                return this.View(listAllVehicleFilterByUserBranchAddress);
            }

            return this.RedirectToAction("Index", "Home");
        }        

        public IActionResult Add()
            => this.View();

        [HttpPost]
        public async Task<IActionResult> Add(AddBranchViewModel branchModel)
        {
            string fullAddress = branchModel.Address + " " + branchModel.Town;
            //TODO: make branch add friendly error page
            if (!ModelState.IsValid)            
            {
                this.TempData.AddFailureMessage(string.Format(FailureAddItemMessage, fullAddress));
                return this.RedirectToAction(nameof(Index));
                //return this.BadRequest();
            }

            await branchesService.AddBranchAsync(branchModel.Town, branchModel.Address);            

            this.TempData.AddSuccessMessage(string.Format(SuccessAddItemMessage, fullAddress));
            return this.RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit([FromRoute(Name = "id")] int branchId)
        {
            var branch = await this.branchesService.GetByIdAsync<BranchDetailViewModel> (branchId);
            if (branch == null) return this.BadRequest();

            var editBranch = new BranchDetailViewModel
            {
                Town = branch.Town,
                Address = branch.Address
            };

            return this.View(editBranch);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BranchDetailViewModel branchModel)
        {
            string fullAddress = branchModel.Address + " " + branchModel.Town;
            //TODO: make branch edit friendly error page
            if (!ModelState.IsValid)
            {
                this.TempData.AddFailureMessage(string.Format(FailureEditItemMessage, fullAddress));
                return this.RedirectToAction(nameof(Index));
                //return this.BadRequest();
            }

            await this.branchesService.EditAsync(branchModel.Id, branchModel.Town, branchModel.Address);

            this.TempData.AddSuccessMessage(string.Format(SuccessEditItemMessage, fullAddress));
            return this.RedirectToAction(nameof(Index));
        }

        
    }
}
