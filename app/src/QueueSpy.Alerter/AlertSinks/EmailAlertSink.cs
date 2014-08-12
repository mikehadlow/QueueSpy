using System;
using EasyNetQ;

namespace QueueSpy.Alerter
{
	public class EmailAlertSink : IAlertSink
	{
		private readonly IBus bus;

		public EmailAlertSink(IBus bus)
		{
			Preconditions.CheckNotNull (bus, "bus");
			this.bus = bus;
		}

		public void Handle (AlertInfo alertInfo)
		{
			var body = string.Format ("QueueSpy Alert!\nUser: {0}\nBroker: {1}\n{2}", 
				alertInfo.User.Email, 
				alertInfo.Broker.Url, 
				alertInfo.Description);

			bus.Publish (new Messages.SendEmailRequest {
				ToAddress = alertInfo.User.Email,
				Subject = "[ALERT!] " + alertInfo.Description,
				Body = body
			});
		}
	}
}

