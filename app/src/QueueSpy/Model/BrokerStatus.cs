namespace QueueSpy
{
	public class BrokerStatus : IModel
	{
		public int Id { get; set; }
		public int BrokerId { get; set; }
		public bool ContactOK { get; set; }
		public string StatusText { get; set; }
	}
}

