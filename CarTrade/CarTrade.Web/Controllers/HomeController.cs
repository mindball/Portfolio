using CarTrade.Services.Branches;
using CarTrade.Services.InsurancePolicies;
using CarTrade.Services.Users;
using CarTrade.Services.Vehicles;
using CarTrade.Services.Vignettes;
using CarTrade.Web.EmailNotifications;
using CarTrade.Web.Filters.Action;
using CarTrade.Web.Models;
using CarTrade.Web.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly IVignettesService vignetteService;
        private readonly IUsersService userService;
        private readonly IEmailService emailService;

        public HomeController(
            ILogger<HomeController> logger,
            IBranchesService branchesService,
            IInsurancesPoliciesService policyService,
            IVehicleService vehicleService,
            IVignettesService vignetteService,
            IEmailService emailService                      
            )
        {
            this.branchesService = branchesService;
            this.policyService = policyService;
            this.vehicleService = vehicleService;
            this.vignetteService = vignetteService;           

            this.emailService = emailService;

            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //this.recurringJobManager.AddOrUpdate(() => this.policyService.SetExpiredInsurancePoliciesLogicAsync(), Cron.Daily);
            //this.recurringJobManager.AddOrUpdate(() => this.vignetteService.SetVignetteExpireLogicAsync(), Cron.Daily);

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

                await this.emailService.ProcessingMessageAsync(collectEpireData);
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
