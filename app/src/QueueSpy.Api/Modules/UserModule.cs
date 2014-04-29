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
			Post ["/"] = _ => RegisterUser(bus, this.Bind<RegisterUserPost>());
		}

		public dynamic RegisterUser(IBus bus, RegisterUserPost user)
		{
			Preconditions.CheckNotNull (user, "user");

			bus.Publish (new Messages.RegisterUser { 
				Email = user.Email,
				Password = user.Password
			});

			return HttpStatusCode.OK;
		}
	}

	public class RegisterUserPost
	{
		public string Email { get; set; }
		public string Password { get; set; }
	}
}

