using System;
using System.Linq;
using System.Net;
using System.Threading;
using Nancy;
using Microsoft.Owin.Hosting;
using Owin;
using EasyNetQ;

namespace QueueSpy.Api
{
	public class Program
    {
		const string url = @"http://+:8080/";

        static void Main(string[] args)
        {
			StaticConfiguration.DisableErrorTraces = false;

			using (WebApp.Start<Startup> (url)) {
				Console.WriteLine ("Nancy listening on {0}. Ctrl-C to stop.", url);
				BlockUntilStopSignal ();
			}

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

	public class QueueSpyApiBootstrapper : DefaultNancyBootstrapper
	{
		protected override void ConfigureApplicationContainer (Nancy.TinyIoc.TinyIoCContainer container)
		{
			base.ConfigureApplicationContainer (container);

			container.Register<IBus> (CreateEasyNetQBus());

			// kick off the hearbeat monitor ...
			container.Resolve<IHeartbeatMonitor> ().Start ();
		}

		public IBus CreateEasyNetQBus()
		{
			var connectionString = System.Configuration.ConfigurationManager.AppSettings ["RabbitMQ"];
			return RabbitHutch.CreateBus (connectionString);
		}
	}
}
