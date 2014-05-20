using System;

namespace QueueSpy.Executor
{
	public class ConnectionLost : IBrokerEventHandler
	{
		private readonly IDbReader dbReader; 
		private readonly IDataWriter dataWriter;

		public ConnectionLost (IDbReader dbReader, IDataWriter dataWriter)
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
				return EventType.ConnectionLost;
			}
		}
	}
}

