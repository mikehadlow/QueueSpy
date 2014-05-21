using System;
using System.Linq;
using EasyNetQ;
using System.Collections.Generic;

namespace QueueSpy.Monitor
{
	public class AnalyseConnections : IAnalysisStep
	{
		private readonly IBus bus;
		private readonly IDbReader dbReader;
		private readonly ILogger logger;

		public AnalyseConnections (IBus bus, IDbReader dbReader, ILogger logger)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (dbReader, "dbReader");
			Preconditions.CheckNotNull (logger, "logger");

			this.bus = bus;
			this.dbReader = dbReader;
			this.logger = logger;
		}

		IDictionary<string, QueueSpy.Connection> LoadConections (int brokerId)
		{
			var connections = new Dictionary<string, QueueSpy.Connection> ();
			foreach (var connection in dbReader.Get<QueueSpy.Connection>("BrokerId = :BrokerId and IsConnected = TRUE", x => x.BrokerId = brokerId)) {
				connections.Add (connection.Name, connection);
			}
			return connections;
		}

		public void Analyse (QueueSpy.Messages.BrokerStatus brokerStatus)
		{
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

