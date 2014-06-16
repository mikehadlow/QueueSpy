using System;

namespace QueueSpy.Messages
{
	public class VHostDeleted : BrokerEvent
	{
		public int VHostId { get; set; }
	}
}

