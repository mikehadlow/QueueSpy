using System;

namespace QueueSpy.Messages
{
	public class Heartbeat
	{
		public string Source { get; set; }
		public DateTime DateTime { get; set; }
	}
}

