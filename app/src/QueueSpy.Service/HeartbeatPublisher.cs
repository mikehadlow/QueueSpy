using System;
using EasyNetQ;

namespace QueueSpy
{
	public interface IHeartbeatPublisher
	{
		void Start();
	}

	public class HeartbeatPublisher : IHeartbeatPublisher, IDisposable
	{
		private readonly IBus bus;
		private readonly string source;
		private readonly System.Threading.Timer timer;

		public HeartbeatPublisher (IBus bus)
		{
			this.bus = bus;
			this.source = System.Reflection.Assembly.GetEntryAssembly().FullName;
			timer = new System.Threading.Timer (OnTimer);
		}

		public void Start()
		{
			timer.Change (0, 5000);
		}

		public void OnTimer(object state)
		{
			try {
				bus.Publish (new QueueSpy.Messages.Heartbeat { Source = source });
			} catch (TimeoutException) {
				// got a timeout exeception, just ignore, the EasyNetQ log will show it anyway.
			}
		}

		#region IDisposable implementation

		public void Dispose ()
		{
			timer.Dispose ();
		}

		#endregion
	}
}

