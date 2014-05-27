using System;

namespace QueueSpy
{
	public class QueueLevel : IModel
	{
		public int Id { get; set; }
		public int BrokerId	{ get; set; }
		public int Ready { get; set; }
		public int Unacked { get; set; }
		public int Total { get; set; }
		public DateTime SampledAt { get; set; }
	}
}

