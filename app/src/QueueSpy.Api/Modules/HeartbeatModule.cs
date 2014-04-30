using System;
using System.Linq;
using System.Collections.Generic;
using Nancy;

namespace QueueSpy.Api
{
	public class HeartbeatModule : NancyModule
	{
		public HeartbeatModule (IHeartbeatMonitor heartbeatMonitor)
		{
			Get ["/heartbeats/"] = parameters => GetAllHeartbeats (heartbeatMonitor);
		}

		public IEnumerable<HeartbeatView> GetAllHeartbeats(IHeartbeatMonitor heartbeatMonitor)
		{
			return heartbeatMonitor.GetListOfHeartbeatServices ().Select (x => new HeartbeatView {
				Source = x.Source,
				ElapsedSeconds = (int)(DateTime.UtcNow - x.DateTime).TotalSeconds
			});
		}
	}

	public class HeartbeatView
	{
		public string Source { get; set; }
		public int ElapsedSeconds { get; set; }
	}
}

