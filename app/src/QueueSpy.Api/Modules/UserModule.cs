using Nancy;

namespace QueueSpy.Api
{
	public class UserModule : NancyModule
	{
		public UserModule (IUserService userService)
		{
			Get ["/user/"] = parameters => userService.GetAllUsers ();
		}
	}
}

