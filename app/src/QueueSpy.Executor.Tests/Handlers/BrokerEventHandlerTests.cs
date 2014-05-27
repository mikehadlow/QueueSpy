using NUnit.Framework;
using Rhino.Mocks;
using EasyNetQ;

namespace QueueSpy.Executor.Tests
{
	[TestFixture ()]
	public class BrokerEventHandlerTests
	{
		private BrokerEventHandler handler;
		private IDataWriter dataWriter;

		[SetUp]
		public void SetUp()
		{
			dataWriter = MockRepository.GenerateStub<IDataWriter> ();
			var container = new TinyIoC.TinyIoCContainer ();
			container.AutoRegister (t => t.Assembly.FullName.StartsWith("QueueSpy"));			
			handler = new BrokerEventHandler (dataWriter, container, new Logger());
		}

		[Test ()]
		[Explicit("Requires database on localhost.")]
		public void Should_insert_brokerEvent ()
		{
			handler.Handle (new QueueSpy.Messages.BrokerEvent { 
				BrokerId = 4,
				EventTypeId = 1,
				DateTimeUTC = System.DateTime.UtcNow,
				Description = "Test broker event."
			});
		}

		[Test]
		public void Should_handle_polymorphic_events ()
		{
			handler.Handle (new Messages.ConsumerCancelled {
				BrokerId = 4,
				EventTypeId = (int)EventType.ConsumerCancelled,
				DateTimeUTC = System.DateTime.UtcNow,
				Description = "test",
				ConsumerId = 101
			});
		}

		[Test]
		[Explicit("Needs a connection to RabbitMQ")]
		public void Should_handle_broker_message_OK ()
		{
			// run Executor, then use this to test message handling ...

			var bus = RabbitHutch.CreateBus ("host=localhost");

			bus.Send (Messages.QueueSpyQueues.CommandQueue, new Messages.ConsumerCancelled {
				BrokerId = 1,
				EventTypeId = (int)EventType.ConsumerCancelled,
				DateTimeUTC = System.DateTime.UtcNow,
				Description = "test",
				ConsumerId = 1
			});
		}
	}
}

