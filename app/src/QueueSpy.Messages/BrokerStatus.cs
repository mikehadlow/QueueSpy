using System;
using System.Linq;
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
		public DateTime SampledAtUtc { get; set; }

		public IList<VHost> VHosts { get; set; }

		public BrokerStatus()
		{
			VHosts = new List<VHost> ();
		}

		public VHost GetOrCreateVHost(string name)
		{
			var vhost = VHosts.SingleOrDefault (x => x.Name == name);
			if(vhost == null) {
				vhost = new VHost { Name = name };
				VHosts.Add (vhost);
			}
			return vhost;
		}
	}

	public class VHost
	{
		public string Name { get; set; }

		public IList<Connection> Connections { get; set; }
		public IList<Queue> Queues { get; set; }

		public VHost ()
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
		public int Ready { get; set; }
		public int Unacked { get; set; }
		public int Total { get; set; }
	}
}

