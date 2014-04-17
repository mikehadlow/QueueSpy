using System;
using System.Threading;
using Nancy.Hosting.Self;
using Nancy;

namespace QueueSpy.Api
{
    class Program
    {
		private static string url = "http://localhost:8080/";

        static void Main(string[] args)
        {
			StaticConfiguration.DisableErrorTraces = false;

			var nancyHost = new NancyHost (new Uri (url));
			nancyHost.Start ();

			var heartbeatMonitor = new HeartbeatMonitor ();
			heartbeatMonitor.Start ();

			Console.WriteLine ("Nancy listening on {0}. Hit <Return> to close.", url);

			var are = new AutoResetEvent (false);

			Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) => {
				// cancel the cancellation to allow the program to shutdown cleanly
				e.Cancel = true;
				are.Set();
			};

			are.WaitOne ();
			nancyHost.Stop ();
			heartbeatMonitor.Stop ();

			Console.WriteLine ("Nancy host closed.");
        }
    }
}
