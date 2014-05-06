using QueueSpy.Messages;

namespace QueueSpy.Executor
{
	public class RegisterUserHandler : ICommandHandler<RegisterUser>
	{
		private readonly IDataWriter dataWriter;
		private readonly IPasswordService passwordService;
		private readonly IMailerService mailerService;

		public RegisterUserHandler (IDataWriter dataWriter, IPasswordService passwordService, IMailerService mailerService)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");
			Preconditions.CheckNotNull (passwordService, "passwordService");
			Preconditions.CheckNotNull (mailerService, "mailerService");

			this.dataWriter = dataWriter;
			this.passwordService = passwordService;
			this.mailerService = mailerService;
		}

		public void Handle(RegisterUser registerUser)
		{
			var hashResult = passwordService.Hash (registerUser.Password);

			var user = new User {
				Email = registerUser.Email,
				PasswordHash = hashResult.PasswordHash,
				Salt = hashResult.Salt
			};

			dataWriter.Insert (user);

			mailerService.Email (registerUser.Email, "Welcome to QueueSpy", "Welcome to QueueSpy (better intro message to follow :)");
		}
	}
}

