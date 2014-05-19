using System;

namespace QueueSpy
{
	public class ClientProperty : IModel
	{
		public int Id { get; set; }
		public int ConnectionId { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
	}
}

