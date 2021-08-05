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
using CarTrade.Web.Infrastructure.Extensions;

namespace CarTrade.Web.Controllers
{
    [AllowAnonymous]
    [TimeFilter]
    public class HomeController : Controller
    {
        private const string FullAddress = "Full address";
        private const string Subject = "Expire data";        
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
            var collectAllUsers = new List<UserWithRoleIdServiceModel>();
            EmailMessage message = new EmailMessage();            
            StringBuilder messageContent = new StringBuilder();            
            messageContent.AppendEmailNewLine(string.Join(": ", FullAddress, branch.FullAddress));

            if (branch.VehiclesWithExpirePolicy.Count > 0)
            {
                collectAllUsers.AddRange(await GetUsersByRoleAsync(branch.BranchId));
                messageContent.AppendEmailNewLine(InsuranceExpire);                

                foreach (var vehicle in branch.VehiclesWithExpirePolicy)
                {                    
                    messageContent.AppendEmailNewLine(string.Join(", ", vehicle.PlateNumber, vehicle.Vin));
                    messageContent.AppendEmailNewLine(string.Join(": ", ExpireData, InsuranceExpire));
                    messageContent.AppendEmailNewLine(string.Join(", ", vehicle.InsurancePolicies
                                .Select(i => new
                                {
                                    TypeOfInsurance = i.TypeInsurance.ToString(),
                                    ExpireDate = i.EndDate
                                })).ToString());
                }
            }

            if (branch.VehiclesWithExpireVignettes.Count > 0)
            {
                collectAllUsers.AddRange(await GetUsersByRoleAsync(branch.BranchId));
                messageContent.AppendEmailNewLine(VignetteExpire);

                foreach (var vehicle in branch.VehiclesWithExpireVignettes)
                {
                    messageContent.AppendEmailNewLine(string.Join(", ", vehicle.PlateNumber, vehicle.Vin));
                    messageContent.AppendEmailNewLine(string.Join(",", vehicle.ExpireDate));
                }              
            }

            if (branch.VehiclesWithInspectionExpire.Count > 0)
            {
                collectAllUsers.AddRange(await GetUsersByRoleAsync(branch.BranchId));
                messageContent.AppendEmailNewLine(InspectionCheckExpire);

                foreach (var vehicle in branch.VehiclesWithInspectionExpire)
                {
                    messageContent.AppendEmailNewLine(string.Join(", ", vehicle.PlateNumber, vehicle.Vin));
                    messageContent.AppendEmailNewLine(string.Join(",", vehicle.InspectionSafetyCheck));
                }              
            }

            if (branch.VehiclesWithOilChangeDistance.Count > 0)
            {
                collectAllUsers.AddRange(await GetUsersByRoleAsync(branch.BranchId));
                messageContent.AppendEmailNewLine(OilCheckExpire);

                foreach (var vehicle in branch.VehiclesWithOilChangeDistance)
                {
                    messageContent.AppendEmailNewLine(string.Join(", ", vehicle.PlateNumber, vehicle.Vin));
                    messageContent.AppendEmailNewLine("Must change oil before " +  vehicle.EndOilChange);
                }                
            }

            if(collectAllUsers.Count() > 0)
            {
                var recipients = RemoveDuplicatesSet(collectAllUsers);                
                message = BuildNotificationMessage(recipients, Subject, messageContent.ToString());
                await this.emailService.Send(message);
            }
        }

        private static List<UserWithRoleIdServiceModel> RemoveDuplicatesSet(List<UserWithRoleIdServiceModel> items)
        {            
            var result = new List<UserWithRoleIdServiceModel>();
            var set = new HashSet<string>();
            foreach (var item in items)
            {
                if (!set.Contains(item.Email))
                {
                    result.Add(item);
                    set.Add(item.Email);
                }
            }
            
            return result;
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

        private async Task<List<UserWithRoleIdServiceModel>> GetUsersByRoleAsync(int branchId)
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
