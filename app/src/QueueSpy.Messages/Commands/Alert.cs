using System;

namespace QueueSpy.Messages
{
	public class Alert
	{
		public int BrokerId { get; set; }
		public int AlertTypeId { get; set; }
		public System.DateTime DateTimeUTC { get; set; }
		public string Description { get; set; }
	}
}

