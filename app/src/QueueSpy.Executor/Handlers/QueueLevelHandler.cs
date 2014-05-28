using System;

namespace QueueSpy.Executor
{
	public class QueueLevelHandler : ICommandHandler<Messages.QueueLevel>
	{
		private readonly IDataWriter dataWriter;

		public QueueLevelHandler (IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.QueueLevel command)
		{
			dataWriter.Insert (new Executor.QueueLevel {
				QueueId = command.QueueId,
				Ready = command.Ready,
				Unacked = command.Unacked,
				Total = command.Total,
				SampledAt = command.SampledAt
			});
		}
	}
}

