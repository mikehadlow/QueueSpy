using System;

namespace QueueSpy.Executor
{
	public class Webhook : IModel
	{
		public int Id { get; set; }
		public string Url { get; set; }
		public int UserId { get; set; }
	}
}

