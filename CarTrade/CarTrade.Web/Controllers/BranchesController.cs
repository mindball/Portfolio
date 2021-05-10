using CarTrade.Services.Branches;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Models.Branches;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    public class BranchesController : BaseController
    {
        private readonly IBranchesService branchesService;
        public BranchesController(IBranchesService branchesService)
        {
            this.branchesService = branchesService;
        }
        public async Task<IActionResult> Index()
        {
            var branches = await this.branchesService.AllAsync();

            return View(new BranchListingViewModel
            {
                Branches = branches
            });
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
            var branch = await this.branchesService.GetByIdAsync(branchId);
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
