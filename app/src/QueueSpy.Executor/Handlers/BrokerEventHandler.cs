using System;

namespace QueueSpy.Executor
{
	public class BrokerEventHandler : ICommandHandler<Messages.BrokerEvent>
	{
		private readonly IDataWriter dataWriter;
		private readonly IDbReader dbReader;

		public BrokerEventHandler (IDataWriter dataWriter, IDbReader dbReader)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");
			Preconditions.CheckNotNull (dbReader, "dbReader");

			this.dataWriter = dataWriter;
			this.dbReader = dbReader;
		}

		public void Handle (QueueSpy.Messages.BrokerEvent command)
		{
			// insert event ...

			var brokerEvent = new QueueSpy.Executor.BrokerEvent {
				BrokerId = command.BrokerId,
				EventTypeId = (EventType)command.EventTypeId,
				DateTimeUTC = command.DateTimeUTC,
				Description = command.Description
			};

			dataWriter.Insert (brokerEvent);

			// update status ...

			var currentStatus = dbReader.GetById<QueueSpy.BrokerStatus> (command.BrokerId);

			if (currentStatus.ContactOK && command.EventTypeId == (int)EventType.BrokerContactLost) {
				dataWriter.Update<Executor.BrokerStatus> (currentStatus.Id, x => {
					x.ContactOK = false;
					x.StatusText = "Contact Lost";
				});
			}

			if (!currentStatus.ContactOK && command.EventTypeId == (int)EventType.BrokerContactEstablished) {
				dataWriter.Update<BrokerStatus> (currentStatus.Id, x => {
					x.ContactOK = true;
					x.StatusText = "OK";
				});
			}
		}
	}
}

