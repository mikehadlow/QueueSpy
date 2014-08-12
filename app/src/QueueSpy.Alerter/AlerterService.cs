using System;
using System.Linq;
using EasyNetQ;
using TinyIoC;
using EasyNetQ.Topology;
using System.Collections.Generic;

namespace QueueSpy.Alerter
{
	public class AlerterService : IQueueSpyService
	{
		private readonly IBus bus;
		private readonly ILogger logger;
		private readonly IDbReader dbReader;

		private readonly IList<IAlertSink> alertSinks = new List<IAlertSink>();

		public AlerterService (IBus bus, ILogger logger, IDbReader dbReader, TinyIoC.TinyIoCContainer container)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (logger, "logger");
			Preconditions.CheckNotNull (dbReader, "dbReader");

			this.bus = bus;
			this.logger = logger;
			this.dbReader = dbReader;

			LoadAlertSinks (container);
		}

		private void LoadAlertSinks(TinyIoC.TinyIoCContainer container)
		{
			var alertSinkInstances =
				from t in GetType ().Assembly.GetTypes ()
				where t.GetInterfaces ().Any (x => x.Name == typeof(IAlertSink).Name)
				select (IAlertSink)container.Resolve (t);

			foreach(var alertSink in alertSinkInstances)
			{
				logger.Log (string.Format("Loaded IAlertSink {0}", alertSink.GetType()));
				alertSinks.Add (alertSink);
			}
		}

		public void Start ()
		{
			SubscribeToBrokerEvents ();
		}

		void SubscribeToBrokerEvents ()
		{
			var queue = bus.Advanced.QueueDeclare (Messages.QueueSpyQueues.AlerterCommands);
			var exchange = bus.Advanced.ExchangeDeclare (Messages.QueueSpyQueues.CommandQueue, ExchangeType.Topic);
			bus.Advanced.Bind (exchange, queue, typeof(Messages.BrokerEvent).Name);

			bus.Advanced.Consume<Messages.BrokerEvent> (queue, (message, info) => {
				HandleBrokerEvent(message.Body);
			});
		}

		void HandleBrokerEvent (QueueSpy.Messages.BrokerEvent brokerEvent)
		{
			logger.Log (string.Format ("Got BrokerEvent {0}", brokerEvent.Description));

			// Very simple initial implementation, just alert on all broker events
			// TODO: Filter on user preferences
			var broker = dbReader.GetById<Broker> (brokerEvent.BrokerId);
			var user = dbReader.GetById<User> (broker.UserId);

			var alertInfo = new AlertInfo {
				Broker = broker,
				User = user,
				AlertType = AlertTypeFromEventTypeId(brokerEvent.EventTypeId),
				DateTimeUTC = brokerEvent.DateTimeUTC,
				Description = brokerEvent.Description
			};

			foreach(var alertSink in alertSinks) {
				alertSink.Handle (alertInfo);
			}
		}

		AlertType AlertTypeFromEventTypeId (int eventTypeId)
		{
			return (AlertType)eventTypeId;
		}
	}

	public interface IAlertSink
	{
		void Handle (AlertInfo alertInfo);
	}

	public class AlertInfo
	{
		public Broker Broker { get; set; }
		public User User { get; set; }
		public AlertType AlertType { get; set; }
		public DateTime DateTimeUTC { get; set; }
		public string Description { get; set; }
	}
}

