using CarTrade.Services.Branches;
using CarTrade.Services.InsurancePolicies;
using CarTrade.Services.Vehicles;
using CarTrade.Web.Filters.Action;
using CarTrade.Web.Models;
using CarTrade.Web.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Web.Controllers
{
    [AllowAnonymous]
    [TimeFilter]
    public class HomeController : Controller
    {        
        private readonly ILogger<HomeController> _logger;
        private readonly IBranchesService branchesService;
        private readonly IInsurancesPoliciesService policyService;
        private readonly IVehicleService vehicleService;        

        public HomeController(
            ILogger<HomeController> logger,
            IBranchesService branchesService,
            IInsurancesPoliciesService policyService,
            IVehicleService vehicleService
            )
        {
            this.branchesService = branchesService;
            this.policyService = policyService;
            this.vehicleService = vehicleService;

            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var allBranchesWithCriticalVehicleData = await this.branchesService.AllAsync();
            if (allBranchesWithCriticalVehicleData.Count() <= 0 || allBranchesWithCriticalVehicleData == null)
            {
                return this.NotFound();
            }

            var expireViewModel = new List<ListExpireDataForAllBranchesViewModel>();

            foreach (var branch in allBranchesWithCriticalVehicleData)
            {
                var collectEpireData = new ListExpireDataForAllBranchesViewModel()
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
                };

                expireViewModel.Add(collectEpireData);                
            }

            //ViewData["NavMenuPage"] = "Index";            

            return View(expireViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
