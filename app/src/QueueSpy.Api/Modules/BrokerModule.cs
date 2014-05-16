using System;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using EasyNetQ;

namespace QueueSpy.Api
{
	public class BrokerModule : NancyModule
	{
		public BrokerModule (IBus bus, IDbReader dbReader) : base("/broker")
		{
			Get ["/"] = _ => dbReader.Get<Broker>("UserId = :UserId", x => x.UserId = this.GetCurrentLoggedInUser ().UserId);

			Get ["/{id}"] = parameters => GetBroker (dbReader, parameters.id);

			Get ["/status/{id}"] = parameters => GetStatus (dbReader, parameters.id);

			Get ["/events/{id}"] = parameters => GetEvents (dbReader, parameters.id);

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

		public dynamic GetBroker (IDbReader dbReader, int id)
		{
			try {
				return RetrieveBroker(dbReader, id);
			} catch (BrokerNotFoundException) {
				return HttpStatusCode.NotFound;
			}
		}

		public dynamic GetEvents (IDbReader dbReader, int id)
		{
			try {
				RetrieveBroker(dbReader, id);
			} catch (BrokerNotFoundException) {
				return HttpStatusCode.NotFound;
			}
			return dbReader.Get<BrokerEvent> ("BrokerId = :BrokerId", x => x.BrokerId = id);
		}

		public dynamic GetStatus (IDbReader dbReader, int id)
		{
			try {
				RetrieveBroker(dbReader, id);
			} catch (BrokerNotFoundException) {
				return HttpStatusCode.NotFound;
			}
			var status = dbReader.Get<BrokerStatus> ("BrokerId = :BrokerId", x => x.BrokerId = id).FirstOrDefault ();
			if (status == null) {
				return HttpStatusCode.NotFound;
			}
			return status;
		}

		public Broker RetrieveBroker(IDbReader dbReader, int id)
		{
			Broker broker = null;
			try {
				broker = dbReader.GetById<Broker> (id);
			} catch (RowNotFoundInTableException) {
				throw new BrokerNotFoundException ();
			}

			var currentUser = this.GetCurrentLoggedInUser ();
			if (currentUser.UserId != broker.UserId) {
				throw new BrokerNotFoundException ();
			}

			return broker;
		}
	}

	
	[Serializable]
	public class BrokerNotFoundException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:BrokerNotFoundException"/> class
		/// </summary>
		public BrokerNotFoundException ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BrokerNotFoundException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public BrokerNotFoundException (string message) : base (message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BrokerNotFoundException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public BrokerNotFoundException (string message, Exception inner) : base (message, inner)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BrokerNotFoundException"/> class
		/// </summary>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected BrokerNotFoundException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
	}
}

