namespace QueueSpy.Executor
{
	public class ChangePasswordHandler : ICommandHandler<Messages.ChangePassword>
	{
		private readonly IDataWriter dataWriter;
		private readonly IPasswordService passwordService;

		public ChangePasswordHandler (IDataWriter dataWriter, IPasswordService passwordService)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");
			Preconditions.CheckNotNull (passwordService, "passwordService");

			this.dataWriter = dataWriter;
			this.passwordService = passwordService;
		}

		public void Handle (QueueSpy.Messages.ChangePassword command)
		{
			var hash = passwordService.Hash (command.NewPassword);
			dataWriter.Update<User> (command.UserId, x => {
				x.PasswordHash = hash.PasswordHash;
				x.Salt = hash.Salt;
			});
		}
	}
}

