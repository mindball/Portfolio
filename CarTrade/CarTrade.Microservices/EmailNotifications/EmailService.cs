using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CarTrade.Microservices.EmailNotifications
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

        public List<EmailMessage> Messages { get; set; }

        public async Task ProcessAndSendingExpireMessages()
        {
            var newMessages = await this.ProcessingMessageAsync();

            if (newMessages != null && newMessages.Count() > 0)
            {
                await this.SendAsync(newMessages);
            }
        }

        public abstract Task<List<EmailMessage>> ProcessingMessageAsync();

        public async Task SendAsync(List<EmailMessage> messages)
        {
            var newEmalMessageFrom = new EmailAddress
            {
                Name = "Car trade notification message",
                Address = emailConfiguration.SmtpUsername
            };

            foreach (var existMessage in messages)
            {
                existMessage.FromAddresses.Add(newEmalMessageFrom);
                var message = new MimeMessage();
                var from = emailConfiguration.SmtpUsername;
                message.To.AddRange(existMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

                message.From.AddRange(existMessage.FromAddresses.Select(x => new MailboxAddress(x.Name = emailConfiguration.SmtpUsername, x.Address = emailConfiguration.SmtpUsername)));

                message.Subject = existMessage.Subject;

                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = existMessage.Content
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
}
