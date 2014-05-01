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
		}

		public dynamic RegisterUser(IBus bus, IUserService userService, RegisterUserPost user)
		{
			Preconditions.CheckNotNull (user, "user");
			Preconditions.CheckNotNull (user.email, "user.Email");
			Preconditions.CheckNotNull (user.password, "user.Password");

			if(userService.UserExists(user.email)) {
				return HttpStatusCode.BadRequest;
			}

			bus.Send (Messages.QueueSpyQueues.CommandQueue, new Messages.RegisterUser { 
				Email = user.email,
				Password = user.password
			});

			return HttpStatusCode.Created;
		}
	}

	public class RegisterUserPost
	{
		public string email { get; set; }
		public string password { get; set; }
	}
}

