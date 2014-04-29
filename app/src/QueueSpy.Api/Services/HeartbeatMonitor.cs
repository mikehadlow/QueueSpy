using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using EasyNetQ;

namespace QueueSpy.Api
{
	public interface IHeartbeatMonitor
	{
		void Start ();
	}

	public class HeartbeatMonitor : IHeartbeatMonitor, IDisposable
	{
		private readonly static HashSet<string> services = new HashSet<string>();
		private readonly IBus bus;

		public HeartbeatMonitor (IBus bus)
		{
			Preconditions.CheckNotNull (bus, "bus");
			this.bus = bus;
		}

		public void Start()
		{
			bus.Subscribe<QueueSpy.Messages.Heartbeat> ("heartbeatMonitor", OnHeartbeat);
		}

		public void OnHeartbeat(QueueSpy.Messages.Heartbeat heartbeat)
		{
			services.Add (heartbeat.Source);
		}

		public static IEnumerable<string> GetListOfHeartbeatServices()
		{
			return services;
		}

		#region IDisposable implementation

		public void Dispose ()
		{
			bus.Dispose ();
		}

		#endregion
	}
}

