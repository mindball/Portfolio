using System.Threading.Tasks;

namespace CarTrade.Microservices.EmailNotifications
{ 
    public interface IEmailService
    {
        public EmailMessage Message { get; set; }

        Task Send(EmailMessage emailMessage);

        Task ProcessingMessageAsync();
    }
}
