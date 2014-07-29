using System;

namespace QueueSpy.Executor
{
	public class AlertHandler : ICommandHandler<Messages.Alert>
	{
		private readonly IDataWriter dataWriter;

		public AlertHandler (IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.Alert command)
		{
			dataWriter.Insert (new Executor.Alert {
				BrokerId = command.BrokerId,
				AlertTypeId = command.AlertTypeId,
				Description = command.Description,
				DateTimeUTC = command.DateTimeUTC
			});
		}
	}
}

