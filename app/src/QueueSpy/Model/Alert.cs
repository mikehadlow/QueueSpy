using System;

namespace QueueSpy
{
	public class Alert : IModel
	{
		public int Id { get; set; }
		public int BrokerId { get; set; }
		public AlertType AlertTypeId { get; set; }
		public string Description { get; set; }
		public System.DateTime DateTimeUTC { get; set; }
	}
}

