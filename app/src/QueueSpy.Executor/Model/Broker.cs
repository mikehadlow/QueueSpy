namespace QueueSpy.Executor
{
	public class Broker : IModel
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string Url { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public bool Active { get; set; }
	}
}

