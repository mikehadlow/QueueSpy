using System;
using System.Linq;

namespace QueueSpy.Executor
{
	public class NewConsumer : IBrokerEventHandler
	{
		private readonly IDataWriter dataWriter;
		private readonly IDbReader dbReader;

		public NewConsumer (IDataWriter dataWriter, IDbReader dbReader)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");
			Preconditions.CheckNotNull (dbReader, "dbReader");

			this.dataWriter = dataWriter;
			this.dbReader = dbReader;
		}

		public EventType EventType {
			get {
				return EventType.NewConsumer;
			}
		}

		public void Handle (QueueSpy.Messages.BrokerEvent brokerEvent)
		{
			var newConsumer = (Messages.NewConsumer)brokerEvent;

			var connection = dbReader.Get<QueueSpy.Connection> ("Name = :Name and IsConnected = TRUE", x => x.Name = newConsumer.ConnectionName)
				.SingleOrDefault ();

			if (connection == null) {
				return;
			}

			dataWriter.Insert<Executor.Consumer> (new Executor.Consumer {
				ConnectionId = connection.Id,
				Tag = newConsumer.Tag,
				QueueName = newConsumer.QueueName,
				Created = newConsumer.DateTimeUTC,
				IsConsuming = true
			});
		}

	}
}

