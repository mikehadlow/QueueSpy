using System;
using System.Linq;

namespace QueueSpy.Executor
{
	public class BrokerContactEstablished : IBrokerEventHandler
	{
		private readonly IDataWriter dataWriter;

		public BrokerContactEstablished (IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.BrokerEvent command)
		{
			dataWriter.Update<Executor.Broker> (command.BrokerId, x => {
				x.ContactOK = true;
			});
		}

		public EventType EventType {
			get {
				return EventType.BrokerContactEstablished;
			}
		}
	}
}

