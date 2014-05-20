using System;
using System.Linq;

namespace QueueSpy.Executor
{
	public class BrokerContactLost : IBrokerEventHandler
	{
		private readonly IDbReader dbReader; 
		private readonly IDataWriter dataWriter;

		public BrokerContactLost (IDbReader dbReader, IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dbReader, "dbReader");
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dbReader = dbReader;
			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.BrokerEvent command)
		{
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

			if (currentStatus.ContactOK) {
				dataWriter.Update<Executor.BrokerStatus> (currentStatus.Id, x => {
					x.ContactOK = false;
					x.StatusText = "Contact Lost";
				});
			}
		}

		public EventType EventType {
			get {
				return EventType.BrokerContactLost;
			}
		}
	}
}

