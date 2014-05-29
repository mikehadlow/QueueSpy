using System;

namespace QueueSpy.Executor
{
	public class ConsumerCancelled : IBrokerEventHandler
	{
		private readonly IDataWriter dataWriter;
		public ConsumerCancelled (IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dataWriter = dataWriter;
		}

		public EventType EventType {
			get {
				return EventType.ConsumerCancelled;
			}
		}

		public void Handle (QueueSpy.Messages.BrokerEvent brokerEvent)
		{
			var consumerCancelled = (Messages.ConsumerCancelled)brokerEvent;
			dataWriter.Update<Executor.Consumer> (consumerCancelled.ConsumerId, x => x.IsConsuming = false);
		}
	}
}
