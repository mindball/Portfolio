using CarTrade.Services.Users;
using CarTrade.Services.Users.Models;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Models.Home;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

using static CarTrade.Web.WebConstants;
using static CarTrade.Common.DataConstants;


namespace CarTrade.Web.EmailNotifications
{
    public class EmailService : IEmailService
    {
        private const string FullAddress = "Full address";
        private const string Subject = "Expire data";
        private const string ExpireData = "Expire date";

		private readonly IEmailConfiguration emailConfiguration;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUsersService userService;


        public EmailService(
            IEmailConfiguration emailConfiguration,
            RoleManager<IdentityRole> roleManager,
            IUsersService userService)
		{
			this.emailConfiguration = emailConfiguration;
            this.roleManager = roleManager;
            this.userService = userService;
		}

        public EmailMessage Message { get; set; }

        public async Task ProcessingMessageAsync(ListExpireDataForAllBranchesViewModel branch)
        {
            var collectAllUsers = new List<UserWithRoleIdServiceModel>();
            
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

        public async Task Send(EmailMessage emailMessage)
        {
            //From
            var newEmalMessageFrom = new EmailAddress
            {
                Name = "Car trade notification message",
                Address = emailConfiguration.SmtpUsername
            };
            emailMessage.FromAddresses.Add(newEmalMessageFrom);

            var message = new MimeMessage();
            var from = emailConfiguration.SmtpUsername;
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

            message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name = emailConfiguration.SmtpUsername, x.Address = emailConfiguration.SmtpUsername)));
            
            message.Subject = emailMessage.Subject;            

            message.Body = new TextPart(TextFormat.Html)
            {
                Text = emailMessage.Content
            };
           
            using (var emailClient = new SmtpClient())
            {
                emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
               
                await emailClient.ConnectAsync(emailConfiguration.SmtpServer, emailConfiguration.SmtpPort, SecureSocketOptions.SslOnConnect);
                                
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                await emailClient.AuthenticateAsync(emailConfiguration.SmtpUsername, emailConfiguration.SmtpPassword);

                await emailClient.SendAsync(message);

                await emailClient.DisconnectAsync(true);
            }

        }
    }
}
