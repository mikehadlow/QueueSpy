using NUnit.Framework;

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
			dataWriter = new DataWriter ();
			handler = new BrokerEventHandler (dataWriter, new TinyIoC.TinyIoCContainer());
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
	}
}

