using CarTrade.Web.Models.Home;
using System.Threading.Tasks;

namespace CarTrade.Web.EmailNotifications
{
    public interface IEmailService
    {
        public EmailMessage Message { get; set; }

        Task Send(EmailMessage emailMessage);

        Task ProcessingMessageAsync();
    }
}
