using System;
using Nancy;
using Nancy.Owin;

namespace QueueSpy.Api
{
	public static class QueueSpyNancyModuleExtensions
	{
		public static CurrentLoggedInUser GetCurrentLoggedInUser(this NancyModule module)
		{
			var env = module.Context.GetOwinEnvironment();
			if (!(env.ContainsKey ("queuespy.email") && env.ContainsKey ("queuespy.userId"))) {
				throw new ApplicationException ("Missing keys: 'queuespy.email' and/or 'queuespy.userId'");
			}
			var email = env ["queuespy.email"].ToString();
			var userId = int.Parse (env ["queuespy.userId"].ToString());
			return new CurrentLoggedInUser (userId, email);
		}
	}

	public class CurrentLoggedInUser
	{
		public CurrentLoggedInUser(int userId, string email)
		{
			UserId = userId;
			Email = email;
		}

		public int UserId { get; private set; }
		public string Email { get; private set; }
	}
}

