using System;
using EasyNetQ;
using TinyIoC;
using EasyNetQ.Topology;

namespace QueueSpy.Alerter
{
	public class AlerterService : IQueueSpyService
	{
		private readonly IBus bus;
		private readonly ILogger logger;

		public AlerterService (IBus bus, ILogger logger)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (logger, "logger");

			this.bus = bus;
			this.logger = logger;
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
		}
	}
}

