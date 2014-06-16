using System;

namespace QueueSpy.Messages
{
	public class VHostCreated : BrokerEvent
	{
		public string Name { get; set; }
	}
}

