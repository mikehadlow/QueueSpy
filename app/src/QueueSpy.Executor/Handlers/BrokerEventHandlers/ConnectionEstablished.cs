using System;

namespace QueueSpy.Executor
{
	public class ConnectionEstablished : IBrokerEventHandler
	{
		private readonly IDbReader dbReader; 
		private readonly IDataWriter dataWriter;

		public ConnectionEstablished (IDbReader dbReader, IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dbReader, "dbReader");
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dbReader = dbReader;
			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.BrokerEvent brokerEvent)
		{
			throw new NotImplementedException ();
		}

		public EventType EventType {
			get {
				return EventType.ConnectionEstablished;
			}
		}
	}
}

