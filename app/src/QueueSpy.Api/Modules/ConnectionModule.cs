using System;
using Nancy;

namespace QueueSpy.Api
{
	public class ConnectionModule : NancyModule
	{
		public ConnectionModule (IBrokerService brokerService) : base("/connection")
		{
			Get ["/{brokerId}"] = parameters => GetConnections (brokerService, parameters.brokerId);
		}

		dynamic GetConnections (IBrokerService brokerService, int brokerId)
		{
			try {
				return brokerService.GetLiveConnections(this.UserId(), brokerId);
			} catch (ItemNotFoundException) {
				return HttpStatusCode.NotFound;
			}
		}
	}
}

