using System;

namespace QueueSpy.Executor
{
	public class ConnectionLost : IBrokerEventHandler
	{
		private readonly IDataWriter dataWriter;

		public ConnectionLost (IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.BrokerEvent brokerEvent)
		{
			var connectionLost = (Messages.ConnectionLost)brokerEvent;
			dataWriter.Update<Executor.Connection> (connectionLost.ConnectionId, x => {
				x.IsConnected = false;
				x.Disconnected = DateTime.UtcNow;
			});
		}

		public EventType EventType {
			get {
				return EventType.ConnectionLost;
			}
		}
	}
}

