using System.Collections.Generic;

namespace QueueSpy
{
	public class VHost : IModel
	{
		public int Id { get; set; }
		public int BrokerId { get; set; }
		public string Name { get; set; }

		public IList<Connection> Connections { get; set; }
		public IList<Queue> Queues { get; set; }

		public VHost ()
		{
			Connections = new List<Connection> ();
			Queues = new List<Queue> ();
		}
	}
}

