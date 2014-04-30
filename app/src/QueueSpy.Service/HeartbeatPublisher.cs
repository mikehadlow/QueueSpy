using System;
using EasyNetQ;

namespace QueueSpy
{
	public class HeartbeatPublisher
	{
		private IBus bus;
		private string source;
		private System.Threading.Timer timer;

		public HeartbeatPublisher (IBus bus, string source)
		{
			this.bus = bus;
			this.source = source;

			timer = new System.Threading.Timer (OnTimer);
			timer.Change (0, 5000);
		}

		public void Stop()
		{
			timer.Dispose ();
		}

		public void OnTimer(object state)
		{
			try {
				bus.Publish (new QueueSpy.Messages.Heartbeat { Source = source });
			} catch (TimeoutException) {
				// got a timeout exeception, just ignore, the EasyNetQ log will show it anyway.
			}
		}
	}
}

