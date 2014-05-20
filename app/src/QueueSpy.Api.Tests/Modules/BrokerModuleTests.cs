using NUnit.Framework;
using Rhino.Mocks;
using System;
using QueueSpy.Api;
using EasyNetQ;

namespace QueueSpy.Api.Tests
{
	[TestFixture ()]
	public class BrokerModuleTests
	{
		private BrokerModule module;
		private IBrokerService dbReader;
		private IBus bus;

		[SetUp]
		public void SetUp()
		{
			dbReader = MockRepository.GenerateStub<IBrokerService> ();
			bus = MockRepository.GenerateStub<IBus> ();
			module = new BrokerModule (bus, dbReader);
			module.SetUser (2, "mike@suteki.co.uk");
		}

		[Test ()]
		[Explicit("Requires a database on localhost")]
		public void GetStatus_should_return_broker_status ()
		{
			module.GetStatus (new BrokerService (new DbReader()), 5);
		}
	}
}

