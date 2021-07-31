using System;
using System.Collections.Generic;
using System.Text;

namespace CarTrade.Web.EmailNotifications
{
    public class EmailService : IEmailService
    {
		private readonly IEmailConfiguration emailConfiguration;

		public EmailService(IEmailConfiguration emailConfiguration)
		{
			this.emailConfiguration = emailConfiguration;
		}		

		public void Send(EmailMessage emailMessage)
		{
			throw new NotImplementedException();
		}
	}
}
