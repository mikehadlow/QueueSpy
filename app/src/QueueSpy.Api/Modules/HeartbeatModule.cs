using System;
using System.Collections.Generic;
using Nancy;

namespace QueueSpy.Api
{
	public class HeartbeatModule : NancyModule
	{
		public HeartbeatModule ()
		{
			Get ["/heartbeats/"] = parameters => GetAllHeartbeats ();
		}

		public IEnumerable<string> GetAllHeartbeats()
		{
			return HeartbeatMonitor.GetListOfHeartbeatServices ();
		}
	}
}

