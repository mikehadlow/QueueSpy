using System.Collections.Generic;
using Nancy;
using Nancy.ModelBinding;
using EasyNetQ;

namespace QueueSpy.Api
{
	public class UserModule : NancyModule
	{
		public UserModule (IUserService userService, IBus bus, IDateService dateService) : base("/user")
		{
			var secretKey = System.Configuration.ConfigurationManager.AppSettings ["SecretKey"];

			Get ["/"] = _ => userService.GetAllUsers ();

			Post ["/"] = _ => RegisterUser(bus, userService, this.Bind<RegisterUserPost>());

			Post ["/changePassword"] = _ => ChangePassword (bus, userService, this.Bind<ChangePasswordPost> ());

			Post ["/cancelAccount"] = _ => CancelAccount (bus);

			Post ["/forgottenPassword"] = _ => ForgottenPassword (bus, userService, this.Bind<ForgottenPasswordPost> ());

			Post ["/passwordReset"] = _ => PasswordReset (bus, dateService, secretKey, this.Bind<PasswordResetPost> ());

		}

		public dynamic RegisterUser(IBus bus, IUserService userService, RegisterUserPost user)
		{
			Preconditions.CheckNotNull (user, "user");
			Preconditions.CheckNotNull (user.email, "user.Email");
			Preconditions.CheckNotNull (user.password, "user.Password");

			if(string.IsNullOrWhiteSpace(user.email)) {
				return Respond.WithBadRequest ("Please enter an email address.");
			}

			if(string.IsNullOrWhiteSpace(user.password) || user.password.Trim().Length < 6)	 {
				return Respond.WithBadRequest ("Please enter a password with at least 6 characters");
			}

			if(userService.UserExists(user.email)) {
				return Respond.WithBadRequest ("Email address '{0}' has already been registered.", user.email);
			}

			bus.SendCommand (new Messages.RegisterUser { 
				Email = user.email,
				Password = user.password
			});

			return HttpStatusCode.Created;
		}

		public dynamic ChangePassword (IBus bus, IUserService userService, ChangePasswordPost changePassword)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (userService, "userService");
			Preconditions.CheckNotNull (changePassword, "changePassword");

			if (string.IsNullOrWhiteSpace (changePassword.oldPassword)) {
				return Respond.WithBadRequest ("Please enter your old password.");
			}

			if (string.IsNullOrWhiteSpace (changePassword.newPassword)) {
				return Respond.WithBadRequest ("Please enter a new password.");
			}

			var currentUser = this.GetCurrentLoggedInUser();
			if (userService.IsValidUser(currentUser.Email, changePassword.oldPassword)) {
				bus.SendCommand(new Messages.ChangePassword {
					UserId = currentUser.UserId,
					NewPassword = changePassword.newPassword
				});
				return HttpStatusCode.OK;
			}

			return Respond.WithBadRequest ("Incorrect Old Password.");
		}

		public dynamic CancelAccount (IBus bus)
		{
			Preconditions.CheckNotNull (bus, "bus");

			var currentUser = this.GetCurrentLoggedInUser ();
			bus.SendCommand (new Messages.CancelAccount { UserId = currentUser.UserId });
			return HttpStatusCode.OK;
		}

		public dynamic ForgottenPassword (IBus bus, IUserService userService, ForgottenPasswordPost forgottenPassword)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (forgottenPassword, "forgottenPassword");

			if (string.IsNullOrWhiteSpace (forgottenPassword.email)) {
				return Respond.WithBadRequest ("Please enter an email address.");
			}
			if (!userService.UserExists (forgottenPassword.email)) {
				return Respond.WithBadRequest ("Unknown email address.");
			}

			var user = userService.GetUserByEmail (forgottenPassword.email);
			bus.SendCommand (new Messages.ForgottenPassword { 
				Email = user.Email,
				UserId = user.Id
			});
			return HttpStatusCode.OK;
		}

		public dynamic PasswordReset (IBus bus, IDateService dateService, string secretKey, PasswordResetPost passwordReset)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (dateService, "dateService");
			Preconditions.CheckNotNull (secretKey, "secretKey");
			Preconditions.CheckNotNull (passwordReset, "passwordReset");

			try {
				var payload = QueueSpy.Authorization.JsonWebToken.DecodeToObject(passwordReset.token, secretKey) as Dictionary<string, object>;
				var exp = (long)payload["exp"];
				var userId = (int)(long)payload["userId"];

				if(dateService.HasExpired(exp)) {
					return Respond.WithBadRequest ("The 30 minutes allowed to reset your password has expired.");
				}

				bus.SendCommand(new Messages.ChangePassword {
					UserId = userId,
					NewPassword = passwordReset.newPassword
				});

				return HttpStatusCode.OK;

			} catch(QueueSpy.Authorization.SignatureVerificationException) {
				return Respond.WithBadRequest ("Invalid Token");
			}
		}
	}

	public class RegisterUserPost
	{
		public string email { get; set; }
		public string password { get; set; }
	}

	public class ForgottenPasswordPost
	{
		public string email { get; set; }
	}

	public static class Respond
	{
		public static Response WithBadRequest(string format, params object[] args)
		{
			var jsonTemplate = "{{ \"message\": \"{0}\" }}";
			var json = string.Format (jsonTemplate, args.Length == 0 ? format : string.Format (format, args));

			var response = (Response)json;
			response.StatusCode = HttpStatusCode.BadRequest;
			return response;
		}
	}
}

