using NUnit.Framework;
using System;
using System.Collections.Generic;
using EasyNetQ;
using Rhino.Mocks;

namespace QueueSpy.Harvester.Tests
{
	[TestFixture ()]
	public class HarvestorServiceTests
	{
		private HarvesterService harvesterService;
		private IBus bus;
		private IDbReader dbReader;

		[SetUp]
		public void SetUp()
		{
			bus = MockRepository.GenerateStub<IBus> ();
			dbReader = MockRepository.GenerateStub<IDbReader> ();

			harvesterService = new HarvesterService (bus, dbReader, new Logger());
		}

		[Test]
		public void BuildBrokerUrl_should_parse_url()
		{
			var url = "https://some.broker.com:123456/";
			var part = harvesterService.BuildBrokerUrl (url);

			Assert.AreEqual (part.HostPart, "https://some.broker.com");
			Assert.AreEqual (part.Port, 123456);
		}

		[Test]
		[Explicit]
		public void OnTimer_should_poll_broker()
		{
			var brokers = new List<Broker> {
				new Broker {
					Url = "http://localhost:15672/",
					Username = "guest",
					Password = "guest",
					Active = true
				}
			};

			dbReader.Stub (x => x.Get<Broker> ("Active = TRUE")).Return (brokers);
			harvesterService.OnTimer (null);

			bus.AssertWasCalled (x => x.Publish(Arg<Messages.BrokerStatus>.Is.Anything));
		}
	}
}

