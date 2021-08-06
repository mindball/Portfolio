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


namespace CarTrade.Web.EmailNotifications
{
    public abstract class EmailService : IEmailService
    {
        private readonly IEmailConfiguration emailConfiguration;

        public EmailService(
            IEmailConfiguration emailConfiguration
            )
		{
			this.emailConfiguration = emailConfiguration;            
		}

        public EmailMessage Message { get; set; }

        public abstract Task ProcessingMessageAsync();        

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
