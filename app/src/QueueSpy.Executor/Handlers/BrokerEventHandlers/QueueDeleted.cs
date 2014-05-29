
namespace QueueSpy.Executor
{
	public class QueueDeleted : IBrokerEventHandler
	{
		private readonly IDataWriter dataWriter;

		public QueueDeleted (IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.BrokerEvent brokerEvent)
		{
			var queueDeleted = (Messages.QueueDeleted)brokerEvent;

			dataWriter.Update<Queue>(queueDeleted.QueueId, x => { 
				x.Deleted = queueDeleted.DateTimeUTC;
				x.IsCurrent = false;
			});
		}

		public EventType EventType {
			get {
				return EventType.QueueDeleted;
			}
		}
	}
}

