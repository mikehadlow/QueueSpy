using System;
using System.Collections.Generic;
using QueueSpy.Authorization;

namespace QueueSpy.Executor
{
	public class ForgottenPasswordHandler : ICommandHandler<Messages.ForgottenPassword>
	{
		private readonly IMailerService mailerService;
		private readonly IDateService dateService;
		private readonly string secretKey;

		public ForgottenPasswordHandler (IMailerService mailerService, IDateService dateService)
		{
			Preconditions.CheckNotNull (mailerService, "mailerService");
			Preconditions.CheckNotNull (dateService, "dateService");

			this.mailerService = mailerService;
			this.dateService = dateService;
			secretKey = System.Configuration.ConfigurationManager.AppSettings ["SecretKey"];
		}

		public void Handle (QueueSpy.Messages.ForgottenPassword command)
		{
			// create a password reset token ...
			var payload = new Dictionary<string, object>{ 
				{ "email", command.Email },
				{ "userId", command.UserId },
				{ "exp", dateService.UnixTimeIn(TimeSpan.FromMinutes(30)) }
			};
			var token = JsonWebToken.Encode(payload, secretKey, JwtHashAlgorithm.HS256);

			var body = string.Format ("Please click the following link to reset your password\n\nhttp://localhost/console/#/password-reset?token={0}", token);
			mailerService.Email (command.Email, "QueueSpy password reset.", body);
		}
	}
}

