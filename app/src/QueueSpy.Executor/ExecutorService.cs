using System.Reflection;
using System.Linq;
using System;
using EasyNetQ;
using EasyNetQ.Consumer;
using TinyIoC;

namespace QueueSpy.Executor
{
	public class ExecutorService : IQueueSpyService
	{
		private readonly IBus bus;
		private readonly TinyIoCContainer container;

		public ExecutorService(IBus bus, TinyIoCContainer container)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (container, "container");

			this.bus = bus;
			this.container = container;
		}

		public void Start()
		{
			bus.Receive (QueueSpy.Messages.QueueSpyQueues.CommandQueue, AddHandlers);
		}

		public void AddHandlers(IReceiveRegistration receiveRegistration)
		{
			// Find all types in this assembly that implement ICommandHandler<T>
			var handlerTypes = 
				from t in this.GetType().Assembly.GetTypes()
				where typeof(ICommandHandler<>).IsAssignableFrom(t)
				let messageType = t.GetInterfaces()[0].GetGenericArguments()[0]
				select new 
					{ 
						HandlerType = t, 
						MessageType = messageType,
						InterfaceType = typeof(ICommandHandler<>).MakeGenericType(messageType)
					};

			// register all discovered handlers with the container
			foreach (var handlerType in handlerTypes) {
				container.Register (handlerType.InterfaceType, handlerType.HandlerType);
			}

			// register handler instances to receive on the command queue
			var addMethod = this.GetType().GetMethod ("AddReceiveRegistration");
			foreach (var handlerType in handlerTypes) {
				var handler = container.Resolve (handlerType.InterfaceType);
				addMethod.MakeGenericMethod (handlerType.MessageType).Invoke (this, new object[] {receiveRegistration, handler});
			}
		}

		// DO NOT DELETE. Discovered by reflection (see AddHandlers above)
		private void AddReceiveRegistration<T>(IReceiveRegistration receiveRegistration, ICommandHandler<T> commandHandler) 
			where T : class
		{
			receiveRegistration.Add ((Action<T>)commandHandler.Handle);
		}
	}

	public interface ICommandHandler<T>
	{
		void Handle(T command);
	}
}
