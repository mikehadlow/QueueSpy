using NUnit.Framework;

namespace QueueSpy.Executor.Tests
{
	[TestFixture ()]
	public class BrokerEventHandlerTests
	{
		private BrokerEventHandler handler;
		private IDataWriter dataWriter;
		private IDbReader dataReader;

		[SetUp]
		public void SetUp()
		{
			dataWriter = new DataWriter ();
			dataReader = new DbReader ();
			handler = new BrokerEventHandler (dataWriter, dataReader, new TinyIoC.TinyIoCContainer());
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

