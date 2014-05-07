using EasyNetQ;

namespace QueueSpy
{
	public static class EasyNetQExtensions
	{
		public static void SendCommand<T>(this IBus bus, T command) where T : class
		{
			bus.Send (QueueSpy.Messages.QueueSpyQueues.CommandQueue, command);
		}
	}
}

