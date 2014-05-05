namespace QueueSpy.Executor
{
	public class User : IModel
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public string Salt { get; set; }
		public bool Active { get; set; }
	}
}

