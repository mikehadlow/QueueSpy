using System;
using Nancy;
using Nancy.ModelBinding;
using EasyNetQ;

namespace QueueSpy.Api
{
	public class BrokerModule : NancyModule
	{
		public BrokerModule (IBus bus, IDbReader dbReader) : base("/broker")
		{
			Get ["/"] = _ => dbReader.Get<Broker>("Id = :Id", x => x.Id = this.GetCurrentLoggedInUser ().UserId);

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
	}
}

