namespace QueueSpy.Executor
{
	public class RegisterBrokerHandler : ICommandHandler<Messages.RegisterBroker>
	{
		private readonly IDataWriter dataWriter;

		public RegisterBrokerHandler (IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.RegisterBroker command)
		{
			var broker = new Broker {
				UserId = command.UserId,
				Url = command.Url,
				Username = command.Username,
				Password = command.Password,
				Active = true
			};

			dataWriter.Insert (broker);
		}
	}
}

