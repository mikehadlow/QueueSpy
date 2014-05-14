namespace QueueSpy.Messages
{
	public class BrokerStatus
	{
		public int BrokerId { get; set; }
		public int UserId { get; set; }
		public string Url { get; set; }
		public bool IsResponding { get; set; }
		public string ErrorMessage { get; set; }
		public string RabbitMQVersion { get; set; }
	}
}

