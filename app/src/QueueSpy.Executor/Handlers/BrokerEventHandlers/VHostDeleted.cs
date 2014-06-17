using System;

namespace QueueSpy.Executor
{
	public class VHostDeleted : IBrokerEventHandler
	{
		private readonly IDataWriter dataWriter;

		public VHostDeleted (IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.BrokerEvent brokerEvent)
		{
			var vhostDeleted = (Messages.VHostDeleted)brokerEvent;

			dataWriter.Update<Executor.VHost> (vhostDeleted.VHostId, x => x.Active = false);
		}

		public EventType EventType {
			get {
				return EventType.VHostDeleted;
			}
		}
	}
}

