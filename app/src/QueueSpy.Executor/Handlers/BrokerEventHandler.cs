using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QueueSpy.Executor
{
	public class BrokerEventHandler : ICommandHandler<Messages.BrokerEvent>
	{
		private readonly IDataWriter dataWriter;
		private readonly ILogger logger;

		private readonly IDictionary<QueueSpy.EventType, IBrokerEventHandler> brokerEventHandlers = 
			new Dictionary<EventType, IBrokerEventHandler> ();

		public BrokerEventHandler (IDataWriter dataWriter, TinyIoC.TinyIoCContainer container, ILogger logger)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");
			Preconditions.CheckNotNull (container, "container");
			Preconditions.CheckNotNull (logger, "loger");

			this.dataWriter = dataWriter;
			this.logger = logger;

			LoadBrokerEventHandlers (container);
		}

		void LoadBrokerEventHandlers (TinyIoC.TinyIoCContainer container)
		{
			var handlers =
				from t in Assembly.GetCallingAssembly ().GetTypes ()
				where t.GetInterfaces ().Any (x => x.Name == typeof(IBrokerEventHandler).Name)
				select (IBrokerEventHandler)container.Resolve(t);
				
			foreach (var handler in handlers) {
				brokerEventHandlers.Add (handler.EventType, handler);
				logger.Log ("Registered Broker Event Handler: {0} for event type: {1}", handler.GetType ().Name, handler.EventType.ToString ());
			}
		}

		public void Handle (QueueSpy.Messages.BrokerEvent command)
		{
			InsertEventRecord (command);
			ExecuteBrokerEventHandler (command);
		}

		public void InsertEventRecord(Messages.BrokerEvent command)
		{
			var brokerEvent = new QueueSpy.Executor.BrokerEvent {
				BrokerId = command.BrokerId,
				EventTypeId = command.EventTypeId,
				DateTimeUTC = command.DateTimeUTC,
				Description = command.Description
			};

			dataWriter.Insert (brokerEvent);
		}

		public void ExecuteBrokerEventHandler(Messages.BrokerEvent command)
		{
			if (!brokerEventHandlers.ContainsKey ((EventType)command.EventTypeId)) {
				throw new QueueSpyServiceException ("No handler for broker event type {0} found.", command.EventTypeId);
			}
			brokerEventHandlers [(EventType)command.EventTypeId].Handle (command);
		}
	}

	public interface IBrokerEventHandler
	{
		EventType EventType { get; }
		void Handle(Messages.BrokerEvent brokerEvent);
	}
}

