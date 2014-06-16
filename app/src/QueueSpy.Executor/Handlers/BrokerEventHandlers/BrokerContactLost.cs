using System;
using System.Linq;

namespace QueueSpy.Executor
{
	public class BrokerContactLost : IBrokerEventHandler
	{
		private readonly IDataWriter dataWriter;

		public BrokerContactLost (IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.BrokerEvent command)
		{
			dataWriter.Update<Executor.Broker> (command.BrokerId, x => {
				x.ContactOK = false;
			});
		}

		public EventType EventType {
			get {
				return EventType.BrokerContactLost;
			}
		}
	}
}

