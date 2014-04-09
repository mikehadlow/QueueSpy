using System;
using Nancy;

namespace QueueSpy.Api
{
	public class HomeModule : NancyModule
	{
		public HomeModule ()
		{
			Get ["/"] = parameters => "Hello from QueueSpy.Api!";
		}
	}
}

