namespace QueueSpy.Messages
{
	public class ConsumerCancelled : BrokerEvent
	{
		public int ConsumerId { get; set; }
	}
}

