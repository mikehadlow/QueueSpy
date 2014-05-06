using System;

namespace QueueSpy.Messages
{
	public class SendEmailRequest
	{
		public string ToAddress { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
	}
}

