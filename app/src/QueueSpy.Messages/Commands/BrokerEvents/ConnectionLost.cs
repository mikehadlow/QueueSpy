using System;

namespace QueueSpy.Messages
{
	public class ConnectionLost : BrokerEvent
	{
		public int ConnectionId { get; set; }
	}
}

