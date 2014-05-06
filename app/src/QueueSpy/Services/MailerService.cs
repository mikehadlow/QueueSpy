using System;
using EasyNetQ;

namespace QueueSpy
{
	public interface IMailerService
	{
		void Email(string toAddress, string subject, string body);
	}

	public class MailerService : IMailerService
	{
		private readonly IBus bus;

		public MailerService (IBus bus)
		{
			this.bus = bus;
		}

		public void Email (string toAddress, string subject, string body)
		{
			Preconditions.CheckNotNull (toAddress, "toAddress");
			Preconditions.CheckNotNull (subject, "subject");
			Preconditions.CheckNotNull (body, "body");

			bus.Publish (new Messages.SendEmailRequest {
				ToAddress = toAddress,
				Subject = subject, 
				Body = body
			});
		}
	}
}

