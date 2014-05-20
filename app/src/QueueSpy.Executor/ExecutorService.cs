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
		private readonly ILogger logger;

		public ExecutorService(IBus bus, TinyIoCContainer container, ILogger logger)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (container, "container");
			Preconditions.CheckNotNull (logger, "logger");

			this.bus = bus;
			this.container = container;
			this.logger = logger;
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
					where t.GetInterfaces ().Any (x => x.Name == typeof(ICommandHandler<>).Name)
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
				logger.Log ("Registered handler: {0} for message type {1}", handlerType.HandlerType.Name, handlerType.MessageType.Name);
			}

			// register handler instances to receive on the command queue
			var addMethod = this.GetType().GetMethod ("AddReceiveRegistration", BindingFlags.NonPublic | BindingFlags.Instance);
			if (addMethod == null) {
				throw new ApplicationException ("Couldn't resolve AddReceiveRegistration`1");
			}

			foreach (var handlerType in handlerTypes) {
				var handler = container.Resolve (handlerType.InterfaceType);
				if (handler == null) {
					throw new ApplicationException (string.Format("Couldn't resolve handler: {0}", handlerType.InterfaceType.Name));
				}
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

	public class TestHandler : ICommandHandler<string>
	{
		public void Handle(string command)
		{
			Console.WriteLine ("Got command {0}");
		}
	}
}
