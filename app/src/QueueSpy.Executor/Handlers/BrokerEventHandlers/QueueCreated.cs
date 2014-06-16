using System;
using System.Linq;

namespace QueueSpy.Executor
{
	public class QueueCreated : IBrokerEventHandler
	{
		private readonly IDataWriter dataWriter;
		private readonly IDbReader dbReader;

		public QueueCreated (IDataWriter dataWriter, IDbReader dbReader)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");
			Preconditions.CheckNotNull (dbReader, "dbReader");

			this.dataWriter = dataWriter;
			this.dbReader = dbReader;
		}

		public void Handle (QueueSpy.Messages.BrokerEvent brokerEvent)
		{
			Preconditions.CheckNotNull (brokerEvent, "brokerEvent");
			var queueCreated = (Messages.QueueCreated)brokerEvent;

			var vhost = dbReader.Get<QueueSpy.VHost> ("Name = :Name", x => x.Name = queueCreated.VHostName).FirstOrDefault ();
			if(vhost == null) {
				return;
			}

			dataWriter.Insert (new Executor.Queue { 
				VHostId = vhost.Id,
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

