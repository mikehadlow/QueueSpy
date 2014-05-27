using System;

namespace QueueSpy.Messages
{
	public class QueueCreated : BrokerEvent
	{
		public string Name { get; set; }
	}
}

