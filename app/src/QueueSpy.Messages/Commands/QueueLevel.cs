using System;

namespace QueueSpy.Messages
{
	public class QueueLevel
	{
		public int QueueId { get; set; }
		public int Ready { get; set; }
		public int Unacked { get; set; }
		public int Total { get; set; }
		public DateTime SampledAt { get; set; }
	}
}

