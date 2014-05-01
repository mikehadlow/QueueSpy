using System;
using QueueSpy.Messages;

namespace QueueSpy.Executor
{
	public class RegisterUserHandler : ICommandHandler<RegisterUser>
	{
		private readonly IDataWriter dataWriter;
		private readonly IPasswordService passwordService;

		public RegisterUserHandler (IDataWriter dataWriter, IPasswordService passwordService)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");
			Preconditions.CheckNotNull (passwordService, "passwordService");

			this.dataWriter = dataWriter;
			this.passwordService = passwordService;
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
		}
	}
}

