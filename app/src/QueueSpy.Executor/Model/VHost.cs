namespace QueueSpy.Executor
{
	public class VHost : IModel
	{
		public int Id { get; set; }
		public int BrokerId { get; set; }
		public string Name { get; set; }
		public bool Active { get; set; }
	}
}

