using Nancy;
using Nancy.ModelBinding;
using EasyNetQ;

namespace QueueSpy.Api
{
	public class UserModule : NancyModule
	{
		public UserModule (IUserService userService, IBus bus) : base("/user")
		{
			Get ["/"] = _ => userService.GetAllUsers ();

			Post ["/"] = _ => RegisterUser(bus, userService, this.Bind<RegisterUserPost>());

			Post ["/changePassword"] = _ => ChangePassword (bus, userService, this.Bind<ChangePasswordPost> ());

			Post ["/cancelAccount"] = _ => CancelAccount (bus);
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

			bus.Send (Messages.QueueSpyQueues.CommandQueue, new Messages.RegisterUser { 
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
				bus.Send(Messages.QueueSpyQueues.CommandQueue, new Messages.ChangePassword {
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
			bus.Send (Messages.QueueSpyQueues.CommandQueue, new Messages.CancelAccount { UserId = currentUser.UserId });
			return HttpStatusCode.OK;
		}
	}

	public class RegisterUserPost
	{
		public string email { get; set; }
		public string password { get; set; }
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

