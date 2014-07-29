using EasyNetQ;
using EasyNetQ.Topology;

namespace QueueSpy
{
	public static class EasyNetQExtensions
	{
		public static void SendCommand<T>(this IBus bus, T command) where T : class
		{
			bus.ConfigureCommandTopology ();
            var wrappedMessage = new Message<T>(command);
			var exchange = new Exchange (QueueSpy.Messages.QueueSpyQueues.CommandQueue);
			bus.Advanced.Publish(exchange, typeof(T).Name, false, false, wrappedMessage);
		}

		private static bool topologyConfigured = false;
		private readonly static object topologyConfigurationLock = new object();

		public static void ConfigureCommandTopology(this IBus bus)
		{
			if (topologyConfigured) return;

			lock (topologyConfigurationLock) {
				if (topologyConfigured) return;

				var queue = bus.Advanced.QueueDeclare (QueueSpy.Messages.QueueSpyQueues.CommandQueue);
				var exchange = bus.Advanced.ExchangeDeclare (QueueSpy.Messages.QueueSpyQueues.CommandQueue, ExchangeType.Topic);
				bus.Advanced.Bind (exchange, queue, "#");
				topologyConfigured = true;
			}
		}
	}
}

