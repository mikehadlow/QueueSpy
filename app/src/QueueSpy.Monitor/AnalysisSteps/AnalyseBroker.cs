using System;
using System.Collections.Generic;
using EasyNetQ;

namespace QueueSpy.Monitor
{
	public class AnalyseBroker : IAnalysisStep
	{
		private readonly IDictionary<int, QueueSpy.BrokerStatus> brokerStatuses = 
			new Dictionary<int, QueueSpy.BrokerStatus> ();

		private readonly IBus bus;
		private readonly IDbReader dbReader;
		private readonly ILogger logger;

		public AnalyseBroker (IBus bus, IDbReader dbReader, ILogger logger)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (dbReader, "dbReader");
			Preconditions.CheckNotNull (logger, "logger");

			this.bus = bus;
			this.dbReader = dbReader;
			this.logger = logger;

			LoadBrokerStatuses ();
		}

		void LoadBrokerStatuses ()
		{
			foreach (var brokerStatus in dbReader.Get<QueueSpy.BrokerStatus>()) {
				brokerStatuses.Add (brokerStatus.BrokerId, brokerStatus);
			}
		}

		public void Analyse (QueueSpy.Messages.BrokerStatus brokerStatus)
		{
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
		}
	}
}

