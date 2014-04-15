using System;
using EasyNetQ;
using System.Threading;
using System.Configuration;

namespace QueueSpy
{
	public class QueuespyApp
	{
		public QueuespyApp ()
		{
		}

		public static void Run(Action<IBus> onStart)
		{
			var are = new AutoResetEvent (false);

			Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) => {
				// cancel the cancellation to allow the program to shutdown cleanly
				e.Cancel = true;
				are.Set();
			};

			var connectionString = System.Configuration.ConfigurationManager.AppSettings ["RabbitMQ"];
			var bus = RabbitHutch.CreateBus (connectionString);
			var heartbeatPublisher = new HeartbeatPublisher (bus, System.Reflection.Assembly.GetEntryAssembly().FullName);

			Console.WriteLine ("Service started. Ctrl-C to stop.");
			are.WaitOne ();

			heartbeatPublisher.Stop ();
			bus.Dispose ();

		}
	}
}

