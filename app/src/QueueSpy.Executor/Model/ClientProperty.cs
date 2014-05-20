using System;

namespace QueueSpy.Executor
{
	public class ClientProperty : IModel
	{
		public int Id { get; set; }
		public int ConnectionId { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
	}
}

