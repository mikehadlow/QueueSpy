namespace QueueSpy.Messages
{
	public class RegisterBroker
	{
		public int UserId { get; set; }
		public string Url { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}
}

