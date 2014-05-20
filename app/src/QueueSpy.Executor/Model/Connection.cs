using System;

namespace QueueSpy.Executor
{
	public class Connection : IModel
	{
		public int Id { get; set; }
		public int BrokerId { get; set; }
		public string Name { get; set; }
		public DateTime Connected { get; set; }
		public DateTime Disconnected { get; set; }
		public bool IsConnected { get; set; }
	}
}

