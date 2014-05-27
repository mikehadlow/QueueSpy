using System.Collections.Generic;

namespace QueueSpy.Messages
{
	public class BrokerStatus
	{
		public int BrokerId { get; set; }
		public int UserId { get; set; }
		public string Url { get; set; }
		public bool IsResponding { get; set; }
		public string ErrorMessage { get; set; }
		public string RabbitMQVersion { get; set; }

		public IList<Connection> Connections { get; set; }
		public IList<Queue> Queues { get; set; }

		public BrokerStatus()
		{
			Connections = new List<Connection> ();
			Queues = new List<Queue> ();
		}
	}

	public class Connection
	{
		public string Name { get; set; }

		public IList<ClientProperty> ClientProperties { get; set; }
		public IList<Consumer> Consumers { get; set; }

		public Connection()
		{
			ClientProperties = new List<ClientProperty> ();
			Consumers = new List<Consumer> ();
		}
	}

	public class ClientProperty
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}

	public class Consumer
	{
		public string Tag { get; set; }
		public string QueueName { get; set; }
	}

	public class Queue
	{
		public string Name { get; set; }
	}
}

