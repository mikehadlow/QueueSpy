using System;
using System.Threading;
using Nancy.Hosting.Self;

namespace QueueSpy.Api
{
    class Program
    {
        static void Main(string[] args)
        {
			var nancyHost = new NancyHost (new Uri ("http://localhost:8888/"));
			nancyHost.Start ();

			Console.WriteLine ("Nancy listening on port 8888. Hit <Return> to close.");

			var are = new AutoResetEvent (false);

			Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) => {
				// cancel the cancellation to allow the program to shutdown cleanly
				e.Cancel = true;
				are.Set();
			};

			are.WaitOne ();
			nancyHost.Stop ();

			Console.WriteLine ("Nancy host closed.");
        }
    }
}
