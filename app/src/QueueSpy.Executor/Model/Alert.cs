using System;

namespace QueueSpy.Executor
{
	public class Alert : IModel
	{
		public int Id { get; set; }
		public int BrokerId { get; set; }
		public int AlertTypeId { get; set; }
		public string Description { get; set; }
		public System.DateTime DateTimeUTC { get; set; }
	}
}

