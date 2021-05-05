using CarTrade.Services.Branches;
using CarTrade.Web.Models.Branches;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Add(AddBranchViewModel branch)
        {
            if(!ModelState.IsValid)
            {
                return this.BadRequest();
            }

            await branchesService.AddBranchAsync(branch.Town, branch.Address);

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
        public async Task<IActionResult> Edit(BranchDetailViewModel brancModel)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }

            await this.branchesService.EditAsync(brancModel.Id, brancModel.Town, brancModel.Address);

            return this.RedirectToAction(nameof(Index));
        }
    }
}
