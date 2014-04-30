using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using EasyNetQ;

namespace QueueSpy.Api
{
	public interface IHeartbeatMonitor
	{
		void Start ();
		IEnumerable<QueueSpy.Messages.Heartbeat> GetListOfHeartbeatServices ();
	}

	public class HeartbeatMonitor : IHeartbeatMonitor, IDisposable
	{
		private readonly ConcurrentDictionary<string, QueueSpy.Messages.Heartbeat> services = new ConcurrentDictionary<string, QueueSpy.Messages.Heartbeat> ();
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
			services.AddOrUpdate (heartbeat.Source, _ => heartbeat, (s, h) => heartbeat);
		}

		public IEnumerable<QueueSpy.Messages.Heartbeat> GetListOfHeartbeatServices()
		{
			return services.Values;
		}

		#region IDisposable implementation

		public void Dispose ()
		{
			bus.Dispose ();
		}

		#endregion
	}
}

