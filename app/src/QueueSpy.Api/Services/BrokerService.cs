using System;
using System.Linq;
using System.Collections.Generic;

namespace QueueSpy.Api
{
	public interface IBrokerService
	{
		IEnumerable<Broker> GetUsersBrokers(int userId);
		Broker GetBroker(int userId, int brokerId);
		IEnumerable<VHost> GetVHosts (int userId, int brokerId);
		IEnumerable<BrokerEvent> GetEvents(int userId, int brokerId);
		IEnumerable<Connection> GetLiveConnections (int userId, int brokerId);
		IEnumerable<Connection> GetDeadConnections (int userId, int brokerId);
		IEnumerable<Queue> GetQueues (int userId, int brokerId);
	}

	public class BrokerService : IBrokerService
	{
		private readonly IDbReader dbReader;

		public BrokerService (IDbReader dbReader)
		{
			Preconditions.CheckNotNull (dbReader, "dbReader");

			this.dbReader = dbReader;
		}

		public IEnumerable<Broker> GetUsersBrokers (int userId)
		{
			return dbReader.Get<Broker>("UserId = :UserId", x => x.UserId = userId);
		}

		public Broker GetBroker (int userId, int brokerId)
		{
			var broker = dbReader.GetById<Broker> (brokerId);
			if (broker == null) {
				throw new ItemNotFoundException ();
			}
			if (broker.UserId != userId) {
				throw new ItemNotFoundException ();
			}
			return broker;
		}

		public IEnumerable<BrokerEvent> GetEvents (int userId, int brokerId)
		{
			GetBroker (userId, brokerId);

			return dbReader.Get<BrokerEvent> ("BrokerId = :BrokerId", x => x.BrokerId = brokerId);
		}

		public IEnumerable<VHost> GetVHosts (int userId, int brokerId)
		{
			GetBroker (userId, brokerId);

			return dbReader.Get<VHost> ("BrokerId = :BrokerId AND Active = TRUE", x => x.BrokerId = brokerId);
		}

		public IEnumerable<Connection> GetLiveConnections (int userId, int brokerId)
		{
			GetBroker (userId, brokerId);

			var whereClause = "BrokerId = :BrokerId AND IsConnected = TRUE";
			Action<dynamic> propertySetter = x => x.BrokerId = brokerId;

			var connections = dbReader.Get<Connection, VHost> (whereClause, propertySetter).ToList ();
			var allClientProperties = dbReader.Get<ClientProperty, Connection, VHost> (whereClause, propertySetter).ToList ();
			var allConsumers = dbReader.Get<Consumer, Connection, VHost> (whereClause + " AND IsConsuming = TRUE", propertySetter).ToList ();

			foreach (var connection in connections) {
				connection.ClientProperties = allClientProperties.Where (x => x.ConnectionId == connection.Id).ToDictionary (x => x.Key, x => x.Value);
				connection.Consumers = allConsumers.Where (x => x.ConnectionId == connection.Id).ToList ();
			}

			return connections;
		}

		public IEnumerable<Connection> GetDeadConnections (int userId, int brokerId)
		{
			GetBroker (userId, brokerId);

			var connections = dbReader.Get<Connection, VHost> ("BrokerId = :BrokerId AND IsConnected = FALSE", x => x.BrokerId = brokerId).ToList ();
			var clientProperties = dbReader.Get<ClientProperty, Connection, VHost> ("BrokerId = :BrokerId AND IsConnected = FALSE", x => x.BrokerId = brokerId).ToList ();

			foreach (var connection in connections) {
				connection.ClientProperties = clientProperties.Where(x => x.ConnectionId == connection.Id).ToDictionary (x => x.Key, x => x.Value);
			}

			return connections;
		}

		public IEnumerable<Queue> GetQueues (int userId, int brokerId)
		{
			GetBroker (userId, brokerId);

			return dbReader.Get<Queue, VHost> ("BrokerId = :BrokerId AND IsCurrent = TRUE", x => x.BrokerId = brokerId).ToList ();
		}
	}

	
	[Serializable]
	public class ItemNotFoundException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ItemNotFoundException"/> class
		/// </summary>
		public ItemNotFoundException ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ItemNotFoundException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public ItemNotFoundException (string message) : base (message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ItemNotFoundException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public ItemNotFoundException (string message, Exception inner) : base (message, inner)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ItemNotFoundException"/> class
		/// </summary>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected ItemNotFoundException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
	}
}

