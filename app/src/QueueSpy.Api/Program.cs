using System;
using System.Threading;
using Nancy;
using Microsoft.Owin.Hosting;
using Owin;

namespace QueueSpy.Api
{
    class Program
    {
		private const string url = "http://localhost:8080/";

        static void Main(string[] args)
        {
			StaticConfiguration.DisableErrorTraces = false;
			var heartbeatMonitor = new HeartbeatMonitor ();

			using (WebApp.Start<Startup> (url)) {
				heartbeatMonitor.Start ();
				Console.WriteLine ("Nancy listening on {0}. Ctrl-C to stop.", url);
				BlockUntilStopSignal ();
			}

			heartbeatMonitor.Stop ();

			Console.WriteLine ("Nancy host closed.");
        }

		static void BlockUntilStopSignal()
		{
			var are = new AutoResetEvent (false);
			Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) => {
				// cancel the cancellation to allow the program to shutdown cleanly
				e.Cancel = true;
				are.Set();
			};
			are.WaitOne ();
		}
    }

	class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app
				.Use(typeof(QueueSpy.Api.Authorization.JwtOwinAuth))
				.UseNancy ();
		}
	}
}
