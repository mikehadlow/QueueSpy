using System;
using System.Collections.Generic;

namespace QueueSpy.Messages
{
	public class ConnectionEstablished : BrokerEvent
	{
		public string Name { get; set; }
		public string VHostName { get; set; }
		public IDictionary<string, string> Properties { get; set; }

		public ConnectionEstablished()
		{
			Properties = new Dictionary<string, string> ();
		}
	}
}

