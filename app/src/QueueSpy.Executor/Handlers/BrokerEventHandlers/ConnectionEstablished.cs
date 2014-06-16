using System;
using System.Linq;

namespace QueueSpy.Executor
{
	public class ConnectionEstablished : IBrokerEventHandler
	{
		private readonly IDataWriter dataWriter;
		private readonly IDbReader dbReader;

		public ConnectionEstablished (IDataWriter dataWriter, IDbReader dbReader)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");
			Preconditions.CheckNotNull (dbReader, "dbReader");

			this.dataWriter = dataWriter;
			this.dbReader = dbReader;
		}

		public void Handle (QueueSpy.Messages.BrokerEvent brokerEvent)
		{
			var connectionEstablished = (Messages.ConnectionEstablished)brokerEvent;

			var vhost = dbReader.Get<QueueSpy.VHost> ("Name = :Name", x => x.Name = connectionEstablished.VHostName).FirstOrDefault ();
			if(vhost == null) {
				return;
			}

			var connectionId = dataWriter.Insert<Executor.Connection> (new Executor.Connection { 
				VHostId = vhost.Id, // TODO:
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

