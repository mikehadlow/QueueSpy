using System;

namespace QueueSpy
{
	public class Queue : IModel
	{
		public int Id { get; set; }
		public int BrokerId { get; set; }
		public string Name { get; set; }
		public DateTime Created { get; set; }
		public DateTime Deleted { get; set; }
		public bool IsCurrent { get; set; }
	}
}

