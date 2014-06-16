using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyNetQ;

namespace QueueSpy.Monitor
{
	public interface IBrokerModelLoader
	{
		Broker LoadBrokerModel (int brokerId);
	}

	public class BrokerModelLoader : IBrokerModelLoader
	{
		private readonly IDbReader dbReader;

		public BrokerModelLoader (IDbReader dbReader)
		{
			Preconditions.CheckNotNull (dbReader, "dbReader");

			this.dbReader = dbReader;
		}

		public Broker LoadBrokerModel (int brokerId)
		{
			var brokerModel = dbReader.GetById<Broker> (brokerId);
			var connections = dbReader.Get<Connection, VHost> ("BrokerId = :BrokerId and IsConnected = TRUE", x => x.BrokerId = brokerId).ToList ();
			var consumers = dbReader.Get<Consumer, Connection, VHost> ("BrokerId = :BrokerId AND IsConsuming = TRUE", x => x.BrokerId = brokerId).ToList ();
			var queues = dbReader.Get<Queue, VHost> ("BrokerId = :BrokerId AND IsCurrent = TRUE", x => x.BrokerId = brokerId).ToList ();

			brokerModel.VHosts = dbReader.Get<VHost> ("BrokerId = :BrokerId", x => x.BrokerId = brokerId).ToList ();

			foreach(var vhost in brokerModel.VHosts) {
				vhost.Connections = connections.Where (x => x.VHostId == vhost.Id).ToList ();
				vhost.Queues = queues.Where (x => x.VHostId == vhost.Id).ToList ();

				foreach(var connection in vhost.Connections) {
					connection.Consumers = consumers.Where (x => x.ConnectionId == connection.Id).ToList ();
				}
			}

			return brokerModel;
		}
	}
}
