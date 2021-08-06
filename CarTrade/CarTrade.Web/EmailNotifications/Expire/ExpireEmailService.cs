using CarTrade.Services.Users;
using CarTrade.Services.Users.Models;
using CarTrade.Web.Models.Home;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static CarTrade.Web.WebConstants;
using static CarTrade.Common.DataConstants;
using CarTrade.Services.Branches;
using System.Text;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Services.Vehicles;

namespace CarTrade.Web.EmailNotifications.Expire
{
    public class ExpireEmailService : EmailService
    {
        private const string FullAddress = "Full address";
        private const string Subject = "Expire data";
        private const string ExpireData = "Expire date";

        private readonly IUsersService userService;
        private readonly IBranchesService branchesService;
        private readonly IVehicleService vehicleService;

        private readonly RoleManager<IdentityRole> roleManager;        

        public ExpireEmailService(
            RoleManager<IdentityRole> roleManager,
            IUsersService userService,
            IEmailConfiguration emailConfiguration,
            IBranchesService branchesService,
            IVehicleService vehicleService)
            : base(emailConfiguration)
        {
            this.roleManager = roleManager;
            this.userService = userService;
            this.branchesService = branchesService;
            this.vehicleService = vehicleService;
        }

        public override async Task ProcessingMessageAsync()
        {
            var allBranchesWithCriticalVehicleData = await this.branchesService.AllAsync();
            if (allBranchesWithCriticalVehicleData.Count() <= 0 || allBranchesWithCriticalVehicleData == null)
            {
                throw new ArgumentException("Missing branches");
            }            

            foreach (var branch in allBranchesWithCriticalVehicleData)
            {
                StringBuilder messageContent = new StringBuilder();
                messageContent.AppendEmailNewLine(string.Join(": ", FullAddress, branch.FullAddress));
                var insurancesExpire = await this.vehicleService.GetInsuranceExpireDataAsync(branch.Id);
                var vignettesExpire = await this.vehicleService.GetVignetteExpireDataAsync(branch.Id);
                var inspectionExpire = await this.vehicleService.GetInspectionSafetyCheckExpireDataAsync(branch.Id);
                var oilExpire = await this.vehicleService.GetOilChangeExpireDataAsync(branch.Id);
                var collectAllUsers = new List<UserWithRoleIdServiceModel>();

                if (insurancesExpire.Count() > 0)
                {
                    collectAllUsers.AddRange(await GetUsersByRoleAsync(branch.Id));
                    messageContent.AppendEmailNewLine(InsuranceExpire);

                    foreach (var vehicle in insurancesExpire)
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

                if (vignettesExpire.Count() > 0)
                {
                    collectAllUsers.AddRange(await GetUsersByRoleAsync(branch.Id));
                    messageContent.AppendEmailNewLine(VignetteExpire);

                    foreach (var vehicle in vignettesExpire)
                    {
                        messageContent.AppendEmailNewLine(string.Join(", ", vehicle.PlateNumber, vehicle.Vin));
                        messageContent.AppendEmailNewLine(string.Join(",", vehicle.ExpireDate));
                    }
                }

                if (inspectionExpire.Count() > 0)
                {
                    collectAllUsers.AddRange(await GetUsersByRoleAsync(branch.Id));
                    messageContent.AppendEmailNewLine(InspectionCheckExpire);

                    foreach (var vehicle in inspectionExpire)
                    {
                        messageContent.AppendEmailNewLine(string.Join(", ", vehicle.PlateNumber, vehicle.Vin));
                        messageContent.AppendEmailNewLine(string.Join(",", vehicle.InspectionSafetyCheck));
                    }
                }

                if (oilExpire.Count() > 0)
                {
                    collectAllUsers.AddRange(await GetUsersByRoleAsync(branch.Id));
                    messageContent.AppendEmailNewLine(OilCheckExpire);

                    foreach (var vehicle in oilExpire)
                    {
                        messageContent.AppendEmailNewLine(string.Join(", ", vehicle.PlateNumber, vehicle.Vin));
                        messageContent.AppendEmailNewLine("Must change oil before " + vehicle.EndOilChange);
                    }
                }

                if (collectAllUsers.Count() > 0)
                {
                    var recipients = RemoveDuplicatesSet(collectAllUsers);                    
                    this.BuildNotificationMessage(recipients, Subject, messageContent.ToString());
                    await this.Send(this.Message);
                }
            }
        }

        private async Task<List<UserWithRoleIdServiceModel>> GetUsersByRoleAsync(int branchId)
        {
            var managerIdRole = await this.roleManager.FindByNameAsync(ManagerRole);
            var userByBranch = this.userService.GetUsersByRole(branchId, managerIdRole.Id);
            return userByBranch;
        }

        private void BuildNotificationMessage(
            List<UserWithRoleIdServiceModel> recipients,
            string subject,
            string content)
        {
            this.Message = new EmailMessage();
            foreach (var recipient in recipients)
            {
                this.Message.ToAddresses.Add(new EmailAddress
                {
                    Name = recipient.Username,
                    Address = recipient.Email
                });
            }

            this.Message.Subject = subject;
            this.Message.Content = content;
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
    }
}
