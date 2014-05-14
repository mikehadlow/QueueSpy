namespace QueueSpy.Messages
{
	public class BrokerEvent
	{
		public int BrokerId { get; set; }
		public int EventTypeId { get; set; }
		public System.DateTime DateTimeUTC { get; set; }
		public string Description { get; set; }
	}
}

