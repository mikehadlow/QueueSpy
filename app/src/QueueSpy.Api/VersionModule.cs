using System;
using System.Reflection;
using Nancy;

namespace QueueSpy.Api
{
	public class VersionModule : NancyModule
	{
		public VersionModule ()
		{
			Get ["/version/"] = parameters => GetVersion ();
		}

		public Version GetVersion()
		{
			return new Version (Assembly.GetExecutingAssembly ().GetName ().Version.ToString ());
		}
	}

	public class Version
	{
		public string Number { get; private set; }

		public Version(string number)
		{
			Number = number;
		}
	}
}

