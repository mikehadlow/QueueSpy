using System;

namespace QueueSpy.Messages
{
	public class QueueDeleted : BrokerEvent
	{
		public int QueueId { get; set; }
	}
}

