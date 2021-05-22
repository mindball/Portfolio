using CarTrade.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using CarTrade.Services.Branches;
using System.Threading.Tasks;
using CarTrade.Web.Models.Home;
using System.Collections.Generic;
using System.Linq;
using CarTrade.Services.InsurancePolicy;
using CarTrade.Services.Vehicle;

namespace CarTrade.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBranchesService branchesService;
        private IInsurancesPoliciesService policyService;
        private IVehicleService vehicleService;

        public HomeController(ILogger<HomeController> logger,
            IBranchesService branchesService,
            IInsurancesPoliciesService policyService,
            IVehicleService vehicleService)
        {
            this.branchesService = branchesService;
            this.policyService = policyService;
            this.vehicleService = vehicleService;

            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var allBranchesWithCriticalVehicleData = await this.branchesService.AllAsync();
            var expireViewModel = new List<ListExpireDataForAllBranchesViewModel>();

            foreach (var branch in allBranchesWithCriticalVehicleData)
            {
                expireViewModel.Add
                (
                    new ListExpireDataForAllBranchesViewModel
                    {
                        BranchId = branch.Id,
                        FullAddress = branch.FullAddress,
                        VehiclesWithExpirePolicy =
                            (await this.vehicleService.GetInsuranceExpireDataAsync(branch.Id)).ToList(),
                        VehiclesWithExpireVignettes =
                            (await this.vehicleService.GetVignetteExpireDataAsync(branch.Id)).ToList(),
                        VehiclesWithOilChangeDistance =
                           (await this.vehicleService.GetOilChangeExpireDataAsync(branch.Id)).ToList(),
                        VehiclesWithInspectionExpire = 
                            (await this.vehicleService.GetInspectionSafetyCheckExpireDataAsync(branch.Id)).ToList()
                    }
                );
            }

            //ViewData["NavMenuPage"] = "Index";            

            return View(expireViewModel);
        }        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
