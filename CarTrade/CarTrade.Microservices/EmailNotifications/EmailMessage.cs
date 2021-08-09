using System.Collections.Generic;

namespace CarTrade.Microservices.EmailNotifications
{
    public class EmailMessage
    {
		public EmailMessage()
		{
			ToAddresses = new List<EmailAddress>();
			FromAddresses = new List<EmailAddress>();
		}

		public List<EmailAddress> ToAddresses { get; set; }

		public List<EmailAddress> FromAddresses { get; set; }

		public string Subject { get; set; }

		public string Content { get; set; }
	}
}
