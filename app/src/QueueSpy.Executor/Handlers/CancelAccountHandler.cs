using System;

namespace QueueSpy.Executor
{
	public class CancelAccountHandler : ICommandHandler<QueueSpy.Messages.CancelAccount>
	{
		private readonly IDataWriter dataWriter;

		public CancelAccountHandler (IDataWriter dataWriter)
		{
			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.CancelAccount command)
		{
			dataWriter.Update<User> (command.UserId, x => x.Active = false);
		}
	}
}

