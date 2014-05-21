using System;

namespace QueueSpy
{
	public class Consumer : IModel
	{
		public int Id { get; set; }
		public int ConnectionId { get; set; }
		public string Tag { get; set; }
		public string QueueName { get; set; }
		public DateTime Created { get; set; }
		public DateTime Cancelled { get; set; }
		public bool IsConsuming { get; set; }
	}
}

