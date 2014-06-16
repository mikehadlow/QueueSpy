using System;
using System.Linq;
using System.Threading;
using EasyNetQ;
using EasyNetQ.Management.Client;
using QueueSpy.Messages;
using System.Collections.Generic;

namespace QueueSpy.Harvester
{
	public class HarvesterService: IQueueSpyService, IDisposable
	{
		private const int pollingIntervalSeconds = 5;
		private readonly IBus bus;
		private readonly IDbReader dbReader;
		private readonly ILogger logger;
		private readonly Timer timer;

		public HarvesterService (IBus bus, IDbReader dbReader, ILogger logger)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (dbReader, "dbReader");
			Preconditions.CheckNotNull (logger, "logger");

			this.bus = bus;
			this.dbReader = dbReader;
			this.logger = logger;
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

				logger.Log ("Querying broker: {0}", broker.Url);

				var status = new Messages.BrokerStatus {
					BrokerId = broker.Id,
					UserId = broker.UserId,
					Url = broker.Url,
					SampledAtUtc = DateTime.UtcNow
				};

				try {
					var part = BuildBrokerUrl (broker.Url);
					var client = new ManagementClient (part.HostPart, broker.Username, broker.Password, part.Port, true);

					try {
						var overview = client.GetOverview ();
						status.IsResponding = true;
						status.RabbitMQVersion = overview.ManagementVersion;

						var connections = client.GetConnections();
						var channels = client.GetChannels ();

						foreach (var connection in connections) {
							var connectionMessage = new Messages.Connection { Name = connection.Name };
							foreach (var property in connection.ClientProperties.PropertiesDictionary) {
								connectionMessage.ClientProperties.Add(new Messages.ClientProperty {
									Key = property.Key,
									Value = property.Value.ToString()
								});
							}
							AddConsumersToConnection(client, channels, connectionMessage);
							status.GetOrCreateVHost(connection.Vhost).Connections.Add(connectionMessage);
						}

						AddQueuesToBroker(client, status);

					} catch (Exception e) {
						status.IsResponding = false;
						status.ErrorMessage = string.Format ("{0}: {1}", e.GetType ().Name, e.Message);
					}

				} catch (UrlParseException ex) {
					status.IsResponding = false;
					status.ErrorMessage = string.Format ("{0}", ex.Message);
				}

				bus.Publish (status);
			}
		}

		void AddConsumersToConnection (ManagementClient client, IEnumerable<EasyNetQ.Management.Client.Model.Channel> channels, QueueSpy.Messages.Connection connection)
		{
			var channelsThatBelongToConnection = channels.Where (x => x.ConnectionDetails.Name == connection.Name);
			foreach (var channel in channelsThatBelongToConnection) {
				var detail = client.GetChannel (channel.Name);
				foreach (var consumer in detail.ConsumerDetails) {
					connection.Consumers.Add (new Messages.Consumer { 
						Tag = consumer.ConsumerTag,
						QueueName = consumer.Queue.Name
					});
				}
			}
		}

		void AddQueuesToBroker (ManagementClient client, QueueSpy.Messages.BrokerStatus brokerStatus)
		{
			var queues = client.GetQueues ();
			foreach(var queue in queues) {
				brokerStatus.GetOrCreateVHost (queue.Vhost).Queues.Add (new Messages.Queue { 
					Name = queue.Name,
					Ready = queue.MessagesReady,
					Unacked = queue.MessagesUnacknowledged,
					Total = queue.Messages
				});
			}
		}

		public UrlPart BuildBrokerUrl (string url)
		{
			var regex = new System.Text.RegularExpressions.Regex ("(https?://[^:]*):([0-9]+)");
			var result = regex.Match (url);
			if (!result.Success) {
				throw new UrlParseException (string.Format ("Failed to parse broker Url: {0}", url));
			}
			if (result.Groups.Count != 3) {
				throw new UrlParseException (string.Format ("URL regex matched, 3 groups were not returned. Url: '{0}'", url));
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

	[Serializable]
	public class UrlParseException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:UrlParseException"/> class
		/// </summary>
		public UrlParseException ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:UrlParseException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public UrlParseException (string message) : base (message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:UrlParseException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public UrlParseException (string message, Exception inner) : base (message, inner)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:UrlParseException"/> class
		/// </summary>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected UrlParseException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
	}
}
