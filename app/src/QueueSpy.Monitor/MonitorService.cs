using System.Collections.Generic;
using EasyNetQ;

namespace QueueSpy.Monitor
{
	public class MonitorService : IQueueSpyService
	{
		private readonly IBus bus;
		private readonly IDbReader dbReader;

		private readonly IDictionary<int, QueueSpy.BrokerStatus> brokerStatuses = 
			new Dictionary<int, QueueSpy.BrokerStatus> ();

		public MonitorService(IBus bus, IDbReader dbReader)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (dbReader, "dbReader");

			this.bus = bus;
			this.dbReader = dbReader;
		}

		public void Start()
		{
			LoadBrokerStatuses ();
			bus.Subscribe<Messages.BrokerStatus> ("monitor", OnBrokerStatus);
		}

		void LoadBrokerStatuses ()
		{
			foreach (var brokerStatus in dbReader.Get<QueueSpy.BrokerStatus>()) {
				brokerStatuses.Add (brokerStatus.Id, brokerStatus);
			}
		}

		void OnBrokerStatus (QueueSpy.Messages.BrokerStatus brokerStatus)
		{
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
				bus.Publish (new Messages.BrokerEvent { 
					BrokerId = brokerStatus.BrokerId,
					EventTypeId = (int)EventType.BrokerContactLost,
					Description = "Broker Contact Lost.",
					DateTimeUTC = System.DateTime.UtcNow
				});
				brokerStatuses [brokerStatus.BrokerId].ContactOK = false;
			}

			if (brokerStatus.IsResponding && !currentStatus.ContactOK) {
				bus.Publish (new Messages.BrokerEvent {
					BrokerId = brokerStatus.BrokerId,
					EventTypeId = (int)EventType.BrokerContactEstablished,
					Description = "Broker Contact Established.",
					DateTimeUTC = System.DateTime.UtcNow
				});
				brokerStatuses [brokerStatus.BrokerId].ContactOK = true;
			}
		}
	}
}
