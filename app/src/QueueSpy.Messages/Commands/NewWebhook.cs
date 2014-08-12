using System;

namespace QueueSpy.Messages
{
	public class NewWebhook
	{
		public int UserId { get; set; }
		public string Url { get; set; }
	}
}

