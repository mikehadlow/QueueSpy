using NUnit.Framework;
using System;

namespace QueueSpy.Api.Tests
{
	[TestFixture ()]
	public class BrokerServiceTests
	{
		private IBrokerService brokerService;

		[SetUp]
		public void SetUp()
		{
			brokerService = new BrokerService (new DbReader ());
		}

		[Test]
		public void ShouldBeAbleToGetConnections()
		{
			var connections = brokerService.GetLiveConnections (2, 4);
			foreach (var item in connections) {
				Console.WriteLine ("{0}", item.Name);
			}
		}
	}
}

