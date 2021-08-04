using System.Threading.Tasks;

namespace CarTrade.Web.EmailNotifications
{
    public interface IEmailService
    {
        public EmailMessage EmailMessage { get; set; }

        Task Send(EmailMessage emailMessage); 
    }
}
