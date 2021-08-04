using CarTrade.Services.Branches;
using CarTrade.Services.InsurancePolicies;
using CarTrade.Services.Users;
using CarTrade.Services.Users.Models;
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CarTrade.Services.Vehicles.Models;

using static CarTrade.Web.WebConstants;
using static CarTrade.Common.DataConstants;
using System.Text;

namespace CarTrade.Web.Controllers
{
    [AllowAnonymous]
    [TimeFilter]
    public class HomeController : Controller
    {
        private const string FullAddress = "Full address";
        private const string VehicleData = "Vehicle";
        private const string ExpireData = "Expire date";

        private readonly ILogger<HomeController> _logger;
        private readonly IBranchesService branchesService;
        private readonly IInsurancesPoliciesService policyService;
        private readonly IVehicleService vehicleService;
        private readonly IVignettesService vignetteService;
        private readonly IUsersService userService;
        private readonly IEmailService emailService;
        private readonly RoleManager<IdentityRole> roleManager;


        //private readonly IRecurringJobManager recurringJobManager;

        public HomeController(
            ILogger<HomeController> logger,
            IBranchesService branchesService,
            IInsurancesPoliciesService policyService,
            IVehicleService vehicleService,
            IVignettesService vignetteService,
            IEmailService emailService,
            IUsersService userService,
            RoleManager<IdentityRole> roleManager
            )
        {
            this.branchesService = branchesService;
            this.policyService = policyService;
            this.vehicleService = vehicleService;
            this.vignetteService = vignetteService;
            this.userService = userService;

            this.emailService = emailService;

            this.roleManager = roleManager;

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

                await CollectManagersSendExpireNotificationAsync(collectEpireData);
            }

            //ViewData["NavMenuPage"] = "Index";            

            return View(expireViewModel);
        }

        private async Task CollectManagersSendExpireNotificationAsync(
            ListExpireDataForAllBranchesViewModel branch)
        {
            StringBuilder messageContent = new StringBuilder();
            if (branch.VehiclesWithExpirePolicy.Count > 0)
            {
                var userByBranch = await GetUsersByRole(branch.BranchId);
                messageContent.AppendLine(InsuranceExpire);
                messageContent.AppendLine(string.Join(": ", FullAddress, branch.FullAddress));
                foreach (var vehicle in branch.VehiclesWithExpirePolicy)
                {
                    messageContent.AppendLine(VehicleData);
                    messageContent.AppendLine(string.Join(", ", vehicle.PlateNumber, vehicle.Vin));
                    messageContent.AppendLine(string.Join(": ", ExpireData, InsuranceExpire));
                    messageContent.AppendLine(string.Join(", ", vehicle.InsurancePolicies
                                .Select(i => new
                                {
                                    TypeOfInsurance = i.TypeInsurance.ToString(),
                                    ExpireDate = i.EndDate
                                })).ToString());
                }
                var message = BuildNotificationMessage(userByBranch, InsuranceExpire, messageContent.ToString());
                await this.emailService.Send(message);
            }

            if (branch.VehiclesWithExpireVignettes.Count > 0)
            {
                var userByBranch = await GetUsersByRole(branch.BranchId);
                //var message = NotificationMessage(userByBranch, branch.VehiclesWithExpireVignettes);
                //await this.emailService.Send(message);
            }

            if (branch.VehiclesWithInspectionExpire.Count > 0)
            {
                var userByBranch = await GetUsersByRole(branch.BranchId);
                //var message = NotificationMessage(userByBranch);
                //await this.emailService.Send(message);
            }

            if (branch.VehiclesWithOilChangeDistance.Count > 0)
            {
                var userByBranch = await GetUsersByRole(branch.BranchId);
                //var message = NotificationMessage(userByBranch);
                //await this.emailService.Send(message);
            }
        }

        private EmailMessage BuildNotificationMessage(
            List<UserWithRoleIdServiceModel> recipients,
            string subject,
            string content)
        {
            var newEmailMessage = new EmailMessage();
            foreach (var recipient in recipients)
            {
                newEmailMessage.ToAddresses.Add(new EmailAddress
                {
                    Name = recipient.Username,
                    Address = recipient.Email
                });
            }

            newEmailMessage.Subject = subject;
            newEmailMessage.Content = content;
            
            return newEmailMessage;
        }

        private async Task<List<UserWithRoleIdServiceModel>> GetUsersByRole(int branchId)
        {
            var managerIdRole = await this.roleManager.FindByNameAsync(ManagerRole);
            var userByBranch = this.userService.GetUsersByRole(branchId, managerIdRole.Id);
            return userByBranch;
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
