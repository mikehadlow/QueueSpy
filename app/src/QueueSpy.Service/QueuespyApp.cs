using System;
using EasyNetQ;
using System.Threading;
using TinyIoC;

namespace QueueSpy
{
	/// <summary>
	/// Common startup method for all QueueSpy components. Sets up container, EasyNetQ IBus, and Hearbeat publisher.
	/// </summary>
	public static class QueuespyApp
	{
		public static void Run()
		{
			var are = new AutoResetEvent (false);

			Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) => {
				// cancel the cancellation to allow the program to shutdown cleanly
				e.Cancel = true;
				are.Set();
			};

			var container = TinyIoCContainer.Current;
			container.AutoRegister (t => t.Assembly.FullName.StartsWith("QueueSpy"));
			container.Register<IBus> (CreateBus ());

			var service = container.Resolve<IQueueSpyService> ();
			service.Start ();

			var heartbeatPublisher = container.Resolve<IHeartbeatPublisher> ();
			heartbeatPublisher.Start ();

			Console.WriteLine ("Service started. Ctrl-C to stop.");
			are.WaitOne ();

			container.Dispose ();
		}

		public static IBus CreateBus()
		{
			var connectionString = System.Configuration.ConfigurationManager.AppSettings ["RabbitMQ"];
			return RabbitHutch.CreateBus (connectionString);
		}
	}


	public interface IQueueSpyService
	{
		void Start();
	}
}

