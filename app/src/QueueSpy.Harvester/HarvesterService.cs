using System;
using System.Threading;
using EasyNetQ;
using EasyNetQ.Management.Client;
using QueueSpy.Messages;

namespace QueueSpy.Harvester
{
	public class HarvesterService: IQueueSpyService, IDisposable
	{
		private const int pollingIntervalSeconds = 5;
		private readonly IBus bus;
		private readonly IDbReader dbReader;
		private readonly Timer timer;

		public HarvesterService (IBus bus, IDbReader dbReader)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (dbReader, "dbReader");

			this.bus = bus;
			this.dbReader = dbReader;
			this.timer = new Timer (OnTimer);
		}

		public void Start()
		{
			timer.Change (TimeSpan.FromSeconds (0), TimeSpan.FromSeconds (pollingIntervalSeconds));
		}

		public void OnTimer (object state)
		{
			var brokers = dbReader.Get<Broker> ("Active = TRUE");

			foreach (var broker in brokers) {
				var status = new BrokerStatus {
						BrokerId = broker.Id,
						UserId = broker.UserId,
						Url = broker.Url,
				};

				var part = BuildBrokerUrl (broker.Url);
				var client = new ManagementClient (part.HostPart, broker.Username, broker.Password, part.Port, true);

				try {
					var overview = client.GetOverview ();
					status.IsResponding = true;
					status.RabbitMQVersion = overview.ManagementVersion;

					bus.Publish (status);

				} catch (Exception e) {
					status.IsResponding = false;
					status.ErrorMessage = string.Format ("{0}: {1}", e.GetType ().Name, e.Message);

					bus.Publish (status);
				}
			}
		}

		public UrlPart BuildBrokerUrl (string url)
		{
			var regex = new System.Text.RegularExpressions.Regex ("(https?://[^:]*):([0-9]+)");
			var result = regex.Match (url);
			if (!result.Success) {
				throw new ApplicationException (string.Format ("Failed to parse broker Url: {0}", url));
			}
			if (result.Groups.Count != 3) {
				throw new ApplicationException (string.Format ("URL regex matched, 3 groups were not returned. Url: '{0}'", url));
			}
			return new UrlPart { HostPart = result.Groups[1].ToString(), Port = int.Parse(result.Groups[2].ToString()) };
		}

		public struct UrlPart
		{
			public string HostPart { get; set; }
			public int Port { get; set; }
		}

		public void Dispose ()
		{
			timer.Dispose ();
		}
	}
}
