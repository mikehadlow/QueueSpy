using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QueueSpy.Executor
{
	public class BrokerEventHandler : ICommandHandler<Messages.BrokerEvent>
	{
		private readonly IDataWriter dataWriter;
		private readonly IDbReader dbReader;
		private readonly IDictionary<QueueSpy.EventType, IBrokerEventHandler> brokerEventHandlers = 
			new Dictionary<EventType, IBrokerEventHandler> ();

		public BrokerEventHandler (IDataWriter dataWriter, IDbReader dbReader, TinyIoC.TinyIoCContainer container)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");
			Preconditions.CheckNotNull (dbReader, "dbReader");

			this.dataWriter = dataWriter;
			this.dbReader = dbReader;

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
			}
		}

		public void Handle (QueueSpy.Messages.BrokerEvent command)
		{
			// insert event ...

			var brokerEvent = new QueueSpy.Executor.BrokerEvent {
				BrokerId = command.BrokerId,
				EventTypeId = command.EventTypeId,
				DateTimeUTC = command.DateTimeUTC,
				Description = command.Description
			};

			dataWriter.Insert (brokerEvent);

			if (!brokerEventHandlers.ContainsKey ((EventType)command.EventTypeId)) {
				throw new QueueSpyServiceException ("No handler for broker event type {0} found.", command.EventTypeId);
			}

			// update status ...
			var currentStatus = dbReader.Get<QueueSpy.BrokerStatus> ("BrokerId = :BrokerId", x => x.BrokerId = command.BrokerId).FirstOrDefault();
			if(currentStatus == null) {
				currentStatus = new QueueSpy.BrokerStatus { 
					ContactOK = false,
					StatusText = "Pending Contact."
				};

				var id = dataWriter.Insert (new Executor.BrokerStatus {
					ContactOK = currentStatus.ContactOK,
					StatusText = currentStatus.StatusText,
					BrokerId = command.BrokerId
				});
				currentStatus.Id = id;
			}

			if (currentStatus.ContactOK && command.EventTypeId == (int)EventType.BrokerContactLost) {
				dataWriter.Update<Executor.BrokerStatus> (currentStatus.Id, x => {
					x.ContactOK = false;
					x.StatusText = "Contact Lost";
				});
			}

			if (!currentStatus.ContactOK && command.EventTypeId == (int)EventType.BrokerContactEstablished) {
				dataWriter.Update<Executor.BrokerStatus> (currentStatus.Id, x => {
					x.ContactOK = true;
					x.StatusText = "OK";
				});
			}
		}
	}

	public interface IBrokerEventHandler
	{
		EventType EventType { get; }
		void Handle(Messages.BrokerEvent brokerEvent);
	}
}

