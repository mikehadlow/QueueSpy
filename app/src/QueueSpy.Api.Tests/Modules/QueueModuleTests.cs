using NUnit.Framework;
using System;

namespace QueueSpy.Api.Tests
{
	[TestFixture ()]
	public class QueueModuleTests
	{
		private QueueModule queueModule;
		private IDbReader dbReader;

		[SetUp]
		public void SetUp()
		{
			dbReader = new DbReader ();
			queueModule = new QueueModule (dbReader);
		}

		[Test, Explicit("Requires database access")]
		public void Should_be_able_to_get_queue_levels ()
		{
			var levels = queueModule.GetQueueLevels (dbReader, 4);
			Console.WriteLine ("Number of level values: {0}", levels.Count);
		}
	}
}

