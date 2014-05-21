using System;
using System.Collections.Generic;

namespace QueueSpy
{
	public class Connection : IModel
	{
		public int Id { get; set; }
		public int BrokerId { get; set; }
		public string Name { get; set; }
		public DateTime Connected { get; set; }
		public DateTime Disconnected { get; set; }
		public bool IsConnected { get; set; }

		public Dictionary<string, string> ClientProperties { get; set; }
		public List<Consumer> Consumers { get; set; }

		public Connection()
		{
			ClientProperties = new Dictionary<string, string> ();
			Consumers = new List<Consumer> ();
		}
	}
}

