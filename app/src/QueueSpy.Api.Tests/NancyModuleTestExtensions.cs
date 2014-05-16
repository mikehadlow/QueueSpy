using System;
using System.Collections.Generic;
using Nancy;

namespace QueueSpy.Api.Tests
{
	public static class NancyModuleTestExtensions
	{
		public static void SetUser(this NancyModule module, int userId, string email)
		{
			if (module.Context == null) {
				module.Context = new NancyContext ();
			}
			module.Context.Items.Add ("OWIN_REQUEST_ENVIRONMENT", new Dictionary<string, object> { 
				{ "queuespy.email", email },
				{ "queuespy.userId", userId }
			});
		}
	}
}

