using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarTrade.Microservices.EmailNotifications
{ 
    public interface IEmailService
    {
        List<EmailMessage> Messages { get; set; }

        Task SendAsync(List<EmailMessage> messages);

        Task<List<EmailMessage>> ProcessingMessageAsync();

        Task ProcessAndSendingExpireMessages();
    }
}
