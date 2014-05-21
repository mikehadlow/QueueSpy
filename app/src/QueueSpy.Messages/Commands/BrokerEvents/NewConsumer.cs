namespace QueueSpy.Messages
{
	public class NewConsumer : BrokerEvent
	{
		public string ConnectionName { get; set; }
		public string Tag { get; set; }
		public string QueueName { get; set; }
	}
}

