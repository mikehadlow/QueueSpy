using System;

namespace QueueSpy.Executor
{
	public class ConnectionEstablished : IBrokerEventHandler
	{
		private readonly IDataWriter dataWriter;

		public ConnectionEstablished (IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.BrokerEvent brokerEvent)
		{
			var connectionEstablished = (Messages.ConnectionEstablished)brokerEvent;
			var connectionId = dataWriter.Insert<Executor.Connection> (new Executor.Connection { 
				BrokerId = connectionEstablished.BrokerId,
				Name = connectionEstablished.Name,
				Connected = connectionEstablished.DateTimeUTC,
				IsConnected = true
			});

			foreach (var clientProperty in connectionEstablished.Properties) {
				dataWriter.Insert<Executor.ClientProperty> (new Executor.ClientProperty { 
					ConnectionId = connectionId,
					Key = clientProperty.Key,
					Value = clientProperty.Value
				});
			}
		}

		public EventType EventType {
			get {
				return EventType.ConnectionEstablished;
			}
		}
	}
}

