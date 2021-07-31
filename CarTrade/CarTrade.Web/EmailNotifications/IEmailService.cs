namespace CarTrade.Web.EmailNotifications
{
    public interface IEmailService
    {
        void Send(EmailMessage emailMessage);        
    }
}
