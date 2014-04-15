using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using EasyNetQ;

namespace QueueSpy.Api
{
	public class HeartbeatMonitor
	{
		private static HashSet<string> services = new HashSet<string>();
		private IBus bus;

		public HeartbeatMonitor ()
		{
		}

		public void Start()
		{
			var connectionString = System.Configuration.ConfigurationManager.AppSettings ["RabbitMQ"];
			bus = RabbitHutch.CreateBus (connectionString);

			bus.Subscribe<QueueSpy.Messages.Heartbeat> ("heartbeatMonitor", OnHeartbeat);
		}

		public void Stop()
		{
			bus.Dispose ();
		}

		public void OnHeartbeat(QueueSpy.Messages.Heartbeat heartbeat)
		{
			services.Add (heartbeat.Source);
		}

		public static IEnumerable<string> GetListOfHeartbeatServices()
		{
			return services;
		}
	}
}

