using System;

namespace QueueSpy.Executor
{
	public class QueueLevel : IModel
	{
		public int Id { get; set; }
		public int QueueId	{ get; set; }
		public int Ready { get; set; }
		public int Unacked { get; set; }
		public int Total { get; set; }
		public DateTime SampledAt { get; set; }
	}
}

