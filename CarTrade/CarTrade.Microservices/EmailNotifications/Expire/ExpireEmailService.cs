using CarTrade.Common.Extensions;
using CarTrade.Data;
using CarTrade.Services.Users;
using CarTrade.Services.Users.Models;
using CarTrade.Services.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarTrade.Common;
using Microsoft.EntityFrameworkCore;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Microservices.EmailNotifications.Expire
{
    public class ExpireEmailService : EmailService
    {
        private const string FullAddress = "Full address";
        private const string Subject = "Expire data";
        private const string ExpireData = "Expire date"; 

        private readonly IUsersService userService;        
        private readonly IVehicleService vehicleService;
        private readonly CarDbContext context;

        public ExpireEmailService(
            CarDbContext context,
            IEmailConfiguration emailConfiguration,            
            IVehicleService vehicleService,
            IUsersService userService
            )
            : base(emailConfiguration)
        {
            this.context = context;
            this.userService = userService;            
            this.vehicleService = vehicleService;
        }

        public override async Task<List<EmailMessage>> ProcessingMessageAsync()
        {
            var allBranchesWithCriticalVehicleData = await this.context.Branches.Select(b => b).ToListAsync();
            var newMessages = new List<EmailMessage>();

            if (allBranchesWithCriticalVehicleData.Count() <= 0 || allBranchesWithCriticalVehicleData == null)
            {
                throw new ArgumentException(NotExistItemExceptionMessage);
            }

            foreach (var branch in allBranchesWithCriticalVehicleData)
            {
                StringBuilder messageContent = new StringBuilder();
                messageContent.AppendEmailNewLine(string.Join(": ", FullAddress, (branch.Town + " " + branch.Address)));

                var insurancesExpire = await this.vehicleService.GetInsuranceExpireDataAsync(branch.Id);
                var vignettesExpire = await this.vehicleService.GetVignetteExpireDataAsync(branch.Id);
                var inspectionExpire = await this.vehicleService.GetInspectionSafetyCheckExpireDataAsync(branch.Id);
                var oilExpire = await this.vehicleService.GetOilChangeExpireDataAsync(branch.Id);
                var collectAllUsers = new List<UserWithRoleIdServiceModel>();

                if (insurancesExpire.Count() > 0)
                {
                    collectAllUsers.AddRange(await GetUsersByRoleAsync(branch.Id));
                    messageContent.AppendEmailNewLine(DataConstants.InsuranceExpire);

                    foreach (var vehicle in insurancesExpire)
                    {  
                        messageContent.AppendEmailNewLine(string.Join(", ", vehicle.PlateNumber, vehicle.Vin));
                        messageContent.AppendEmailNewLine(string.Join(": ", ExpireData, DataConstants.InsuranceExpire));
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
                    messageContent.AppendEmailNewLine(DataConstants.VignetteExpire);

                    foreach (var vehicle in vignettesExpire)
                    {                        
                        messageContent.AppendEmailNewLine(string.Join(", ", vehicle.PlateNumber, vehicle.Vin));
                        messageContent.AppendEmailNewLine(string.Join(",", vehicle.ExpireDate));
                    }
                }

                if (inspectionExpire.Count() > 0)
                {
                    collectAllUsers.AddRange(await GetUsersByRoleAsync(branch.Id));
                    messageContent.AppendEmailNewLine(DataConstants.InspectionCheckExpire);

                    foreach (var vehicle in inspectionExpire)
                    {                        
                        messageContent.AppendEmailNewLine(string.Join(", ", vehicle.PlateNumber, vehicle.Vin));
                        messageContent.AppendEmailNewLine(string.Join(",", vehicle.InspectionSafetyCheck));
                    }
                }

                if (oilExpire.Count() > 0)
                {
                    collectAllUsers.AddRange(await GetUsersByRoleAsync(branch.Id));
                    messageContent.AppendEmailNewLine(DataConstants.OilCheckExpire);

                    foreach (var vehicle in oilExpire)
                    {                       
                        messageContent.AppendEmailNewLine(string.Join(", ", vehicle.PlateNumber, vehicle.Vin));
                        messageContent.AppendEmailNewLine("Must change oil before " + vehicle.EndOilChange);
                    }
                }

                //If branch has no manager
                if (collectAllUsers.Count() > 0 
                    || (oilExpire.Count() 
                    + inspectionExpire.Count() 
                    + vignettesExpire.Count() 
                    + insurancesExpire.Count()) > 0)
                {
                    if(this.Messages == null) this.Messages = new List<EmailMessage>();

                    var recipients = await RemoveDuplicatesSet(collectAllUsers);
                    this.Messages.Add(this.BuildNotificationMessage(recipients, Subject, messageContent.ToString()));
                }                
            }

            return this.Messages;
        }

        private async Task<List<UserWithRoleIdServiceModel>> GetUsersByRoleAsync(int branchId)
        {
            var managerIdRole = await this.context.Roles
                .Where(r => r.Name == DataConstants.ManagerRole)
                .FirstOrDefaultAsync();
            var userByBranch = this.userService.GetUsersByRole(branchId, managerIdRole.Id);
            return userByBranch;
        }

        private EmailMessage BuildNotificationMessage(
            List<UserWithRoleIdServiceModel> recipients,
            string subject,
            string content)
        {
            var newMessage = new EmailMessage();
            foreach (var recipient in recipients)
            {
                newMessage.ToAddresses.Add(new EmailAddress
                {
                    Name = recipient.Username,
                    Address = recipient.Email
                });
            }

            newMessage.Subject = subject;
            newMessage.Content = content;

            return newMessage;
        }

        private async Task<List<UserWithRoleIdServiceModel>> RemoveDuplicatesSet(List<UserWithRoleIdServiceModel> items)
        {
            //If branch has no manager
            var result = new List<UserWithRoleIdServiceModel>();
            if (items.Count == 0)
            {
                var adminRole = await this.context.Roles
               .Where(r => r.Name == DataConstants.AdministratorRole)
               .FirstOrDefaultAsync();
                var adminUserId = await this.context.UserRoles.Where(u => u.RoleId == adminRole.Id).Select(u => u.UserId).FirstOrDefaultAsync();
                var admin = await this.context.Users.FirstOrDefaultAsync(u => u.Id == adminUserId);

                result.Add(new UserWithRoleIdServiceModel
                {
                    Username = admin.UserName,
                    Email = admin.Email,
                    RoleId = adminRole.Id                    
                });

                return result;
            }
            
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
