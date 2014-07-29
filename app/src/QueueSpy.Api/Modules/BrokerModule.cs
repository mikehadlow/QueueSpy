using System;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using EasyNetQ;

namespace QueueSpy.Api
{
	public class BrokerModule : NancyModule
	{
		public BrokerModule (IBus bus, IBrokerService brokerService) : base("/broker")
		{
			Get ["/"] = _ => brokerService.GetUsersBrokers (this.UserId());

			Get ["/{id}"] = parameters => GetBroker (brokerService, parameters.id);

			Get ["/vhosts/{id}"] = parameters => GetVHosts (brokerService, parameters.id);

			Get ["/events/{id}"] = parameters => GetEvents (brokerService, parameters.id);

			Get ["/alerts/{id}"] = parameters => GetAlerts (brokerService, parameters.id);

			Get ["/queues/{id}"] = parameters => GetQueues (brokerService, parameters.id);

			Post ["/"] = _ => RegisterBroker (bus, this.Bind<RegisterBrokerPost>());
		}

		public dynamic RegisterBroker (IBus bus, RegisterBrokerPost registerBroker)
		{
			Preconditions.CheckNotNull (registerBroker, "registerBroker");

			if (string.IsNullOrWhiteSpace (registerBroker.url)) {
				return Respond.WithBadRequest ("You must enter your Broker's URL");
			}
			if (string.IsNullOrWhiteSpace (registerBroker.username)) {
				return Respond.WithBadRequest ("You must enter the user name for your Broker.");
			}
			if (string.IsNullOrWhiteSpace (registerBroker.password)) {
				return Respond.WithBadRequest ("You must enter a password for your Broker.");
			}

			var registerBrokerMessage = new Messages.RegisterBroker {
				UserId = this.GetCurrentLoggedInUser ().UserId,
				Url = registerBroker.url,
				Username = registerBroker.username,
				Password = registerBroker.password
			};

			bus.SendCommand (registerBrokerMessage);

			return HttpStatusCode.OK;
		}

		public dynamic GetBroker (IBrokerService brokerService, int id)
		{
			try {
				return brokerService.GetBroker(this.UserId(), id);
			} catch (ItemNotFoundException) {
				return HttpStatusCode.NotFound;
			}
		}

		public dynamic GetVHosts (IBrokerService brokerService, int id)
		{
			try {
				return brokerService.GetVHosts(this.UserId(), id);
			} catch (ItemNotFoundException) {
				return HttpStatusCode.NotFound;
			}
		}

		public dynamic GetEvents (IBrokerService brokerService, int id)
		{
			try {
				return brokerService.GetEvents(this.UserId(), id);
			} catch (ItemNotFoundException) {
				return HttpStatusCode.NotFound;
			}
		}

		public dynamic GetAlerts (IBrokerService brokerService, int id)
		{
			try {
				return brokerService.GetAlerts(this.UserId(), id);
			} catch (ItemNotFoundException) {
				return HttpStatusCode.NotFound;
			}
		}

		public dynamic GetQueues (IBrokerService brokerService, int id)
		{
			try {
				return brokerService.GetQueues(this.UserId(), id);
			} catch (ItemNotFoundException) {
				return HttpStatusCode.NotFound;
			}
		}
	}
}

