using System;

namespace QueueSpy.Executor
{
	public class VHostCreated : IBrokerEventHandler
	{
		private readonly IDataWriter dataWriter;

		public VHostCreated (IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.BrokerEvent brokerEvent)
		{
			var vhostCreated = (Messages.VHostCreated)brokerEvent;

			dataWriter.Insert<Executor.VHost> (new VHost {
				BrokerId = vhostCreated.BrokerId,
				Name = vhostCreated.Name,
				Active = true
			});
		}

		public EventType EventType {
			get {
				return EventType.VHostCreated;
			}
		}
	}
}

