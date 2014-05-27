using System;

namespace QueueSpy.Executor
{
	public class QueueCreated : IBrokerEventHandler
	{
		private readonly IDataWriter dataWriter;

		public QueueCreated (IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.BrokerEvent brokerEvent)
		{
			Preconditions.CheckNotNull (brokerEvent, "brokerEvent");
			var queueCreated = (Messages.QueueCreated)brokerEvent;

			dataWriter.Insert (new Executor.Queue { 
				BrokerId = queueCreated.BrokerId,
				Name = queueCreated.Name,
				Created = queueCreated.DateTimeUTC,
				IsCurrent = true
			});
		}

		public EventType EventType {
			get {
				return EventType.QueueCreated;
			}
		}
	}
}

