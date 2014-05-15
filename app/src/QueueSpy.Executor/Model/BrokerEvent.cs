namespace QueueSpy.Executor
{
	public class BrokerEvent : IModel
	{
		public int Id { get; set; }
		public int BrokerId { get; set; }
		public int EventTypeId { get; set; }
		public string Description { get; set; }
		public System.DateTime DateTimeUTC { get; set; }
	}
}

