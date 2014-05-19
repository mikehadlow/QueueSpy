using System.Collections.Generic;
using System.Linq;
using EasyNetQ;

namespace QueueSpy.Monitor
{
	public class MonitorService : IQueueSpyService
	{
		private readonly IBus bus;
		private readonly IDbReader dbReader;
		private readonly ILogger logger;

		private readonly IDictionary<int, QueueSpy.BrokerStatus> brokerStatuses = 
			new Dictionary<int, QueueSpy.BrokerStatus> ();

		public MonitorService(IBus bus, IDbReader dbReader, ILogger logger)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (dbReader, "dbReader");
			Preconditions.CheckNotNull (logger, "logger");

			this.bus = bus;
			this.dbReader = dbReader;
			this.logger = logger;
		}

		public void Start()
		{
			LoadBrokerStatuses ();
			bus.Subscribe<Messages.BrokerStatus> ("monitor", OnBrokerStatus);
		}

		void LoadBrokerStatuses ()
		{
			foreach (var brokerStatus in dbReader.Get<QueueSpy.BrokerStatus>()) {
				brokerStatuses.Add (brokerStatus.BrokerId, brokerStatus);
			}
		}

		IDictionary<string, QueueSpy.Connection> LoadConections (int brokerId)
		{
			var connections = new Dictionary<string, QueueSpy.Connection> ();
			foreach (var connection in dbReader.Get<QueueSpy.Connection>("BrokerId = :BrokerId and IsConnected = TRUE", x => x.BrokerId = brokerId)) {
				connections.Add (connection.Name, connection);
			}
			return connections;
		}

		void OnBrokerStatus (QueueSpy.Messages.BrokerStatus brokerStatus)
		{
			logger.Log ("BrokerStatus received, id: {0}.", brokerStatus.BrokerId);

			// establish if broker has just been connected or disconnected.
			var currentStatus = new QueueSpy.BrokerStatus { 
				BrokerId = brokerStatus.BrokerId,
				ContactOK = false
			};

			if (brokerStatuses.ContainsKey (brokerStatus.BrokerId)) {
				currentStatus = brokerStatuses [brokerStatus.BrokerId];
			} else {
				brokerStatuses.Add (brokerStatus.BrokerId, currentStatus);
			}

			if (!brokerStatus.IsResponding && currentStatus.ContactOK) {
				bus.SendCommand (new Messages.BrokerEvent { 
					BrokerId = brokerStatus.BrokerId,
					EventTypeId = (int)EventType.BrokerContactLost,
					Description = "Broker Contact Lost.",
					DateTimeUTC = System.DateTime.UtcNow
				});
				brokerStatuses [brokerStatus.BrokerId].ContactOK = false;
				logger.Log ("Broker, id: {0}, Lost Contact.", brokerStatus.BrokerId);
			}

			if (brokerStatus.IsResponding && !currentStatus.ContactOK) {
				bus.SendCommand (new Messages.BrokerEvent {
					BrokerId = brokerStatus.BrokerId,
					EventTypeId = (int)EventType.BrokerContactEstablished,
					Description = "Broker Contact Established.",
					DateTimeUTC = System.DateTime.UtcNow
				});
				brokerStatuses [brokerStatus.BrokerId].ContactOK = true;
				logger.Log ("Broker, id: {0}, Contact Established.", brokerStatus.BrokerId);
			}

			// find any new or missing connections
			if (brokerStatus.IsResponding) {
				var connections = LoadConections (brokerStatus.BrokerId);

				// new connections
				var newConnections = brokerStatus.Connections.Where (x => !connections.ContainsKey (x.Name));
				foreach (var newConnection in newConnections) {
					bus.SendCommand<Messages.BrokerEvent> (new Messages.ConnectionEstablished { 
						BrokerId = brokerStatus.BrokerId,
						EventTypeId = (int)EventType.ConnectionEstablished,
						Description = string.Format("Connection '{0}' Established.", newConnection.Name),
						DateTimeUTC = System.DateTime.UtcNow,
						Name = newConnection.Name,
						Properties = newConnection.ClientProperties.ToDictionary(x => x.Key, x => x.Value)
					});
				}

				// lost connections
				var lostConnections = connections.Where (x => brokerStatus.Connections.All (y => y.Name != x.Key)).Select (x => x.Value);
				foreach (var lostConnection in lostConnections) {
					bus.SendCommand<Messages.BrokerEvent> (new Messages.ConnectionLost {
						BrokerId = brokerStatus.BrokerId,
						EventTypeId = (int)EventType.ConnectionLost,
						Description = string.Format ("Connection '{0}' Lost.", lostConnection.Name),
						DateTimeUTC = System.DateTime.UtcNow,
						ConnectionId = lostConnection.Id
					});
				}
			}
		}
	}
}
