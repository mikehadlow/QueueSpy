using System;

namespace QueueSpy
{
	public class QueueLevel : IModel
	{
		public int Id { get; set; }
		public int QueueId	{ get; set; }
		public long Ready { get; set; }
		public long Unacked { get; set; }
		public long Total { get; set; }
		public DateTime SampledAt { get; set; }
	}
}

