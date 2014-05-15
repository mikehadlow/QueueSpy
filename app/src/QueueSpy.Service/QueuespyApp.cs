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
			// load the QueueSpy assembly into the app domain.
			var name = typeof(QueueSpy.IPasswordService).Name;

			var are = new AutoResetEvent (false);

			Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) => {
				// cancel the cancellation to allow the program to shutdown cleanly
				e.Cancel = true;
				are.Set();
			};

			var container = TinyIoCContainer.Current;
			container.AutoRegister (t => t.Assembly.FullName.StartsWith("QueueSpy"));
			container.Register<IEasyNetQLogger, EasyNetQLogger> ();

			var easyNetQLogger = container.Resolve<IEasyNetQLogger> ();
			var logger = container.Resolve<ILogger> ();

			container.Register<IBus> (CreateBus (easyNetQLogger));

			var service = container.Resolve<IQueueSpyService> ();
			service.Start ();

			var heartbeatPublisher = container.Resolve<IHeartbeatPublisher> ();
			heartbeatPublisher.Start ();

			logger.Log ("Service started. Ctrl-C to stop.");
			are.WaitOne ();
			logger.Log ("Service stopping.");

			container.Dispose ();
			logger.Log ("Service stopped.");
		}

		public static IBus CreateBus(IEasyNetQLogger easyNetQLogger)
		{
			var connectionString = System.Configuration.ConfigurationManager.AppSettings ["RabbitMQ"];
			return RabbitHutch.CreateBus (connectionString, x => x.Register<IEasyNetQLogger>(_ => easyNetQLogger));
		}
	}

	public interface IQueueSpyService
	{
		void Start();
	}

	public class EasyNetQLogger : IEasyNetQLogger
	{
		private readonly ILogger logger;

		public EasyNetQLogger(ILogger logger)
		{
			this.logger = logger;
		}

		public void DebugWrite (string format, params object[] args)
		{
			// no debug
		}

		public void InfoWrite (string format, params object[] args)
		{
			logger.Log ("[EasyNetQ-INFO] " + format, args);
		}

		public void ErrorWrite (string format, params object[] args)
		{
			logger.Log ("[EasyNetQ-ERROR] " + format, args);
		}

		public void ErrorWrite (Exception exception)
		{
			logger.Log ("[EasyNetQ-ERROR] {0}", exception.ToString());
		}
	}
}

