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
using CarTrade.Services.InsurancePolicies;
using CarTrade.Services.Vehicles;
using Hangfire;
using CarTrade.Services.Vignettes;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Filters.Action;

namespace CarTrade.Web.Controllers
{
    [AllowAnonymous]
    [TimeFilter]
    public class  HomeController: Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBranchesService branchesService;
        private IInsurancesPoliciesService policyService;
        private IVehicleService vehicleService;
        private IVignettesService vignetteService;

        //private readonly IRecurringJobManager recurringJobManager;

        public HomeController(
            ILogger<HomeController> logger,
            IBranchesService branchesService,
            IInsurancesPoliciesService policyService,
            IVehicleService vehicleService,
            IVignettesService vignetteService
            //IRecurringJobManager recurringJobManager
            )
        {
            this.branchesService = branchesService;
            this.policyService = policyService;
            this.vehicleService = vehicleService;
            this.vignetteService = vignetteService;

            //this.recurringJobManager = recurringJobManager;

            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //this.recurringJobManager.AddOrUpdate(() => this.policyService.SetExpiredInsurancePoliciesLogicAsync(), Cron.Daily);
            //this.recurringJobManager.AddOrUpdate(() => this.vignetteService.SetVignetteExpireLogicAsync(), Cron.Daily);
                       
            var allBranchesWithCriticalVehicleData = await this.branchesService.AllAsync();
            if(allBranchesWithCriticalVehicleData.Count() <= 0 || allBranchesWithCriticalVehicleData == null)
            {
                return this.NotFound();
            }

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
