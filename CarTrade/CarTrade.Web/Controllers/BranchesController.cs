using CarTrade.Services.Branches;
using CarTrade.Web.Models.Branches;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Web.Controllers
{
    public class BranchesController : Controller
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

        public async Task<IActionResult> Add()
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
    }
}
