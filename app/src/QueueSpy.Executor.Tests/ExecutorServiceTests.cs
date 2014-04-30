using NUnit.Framework;
using System;

namespace QueueSpy.Executor.Tests
{
	[TestFixture ()]
	public class ExecutorServiceTests
	{
		ExecutorService executorService;

		[SetUp]
		public void SetUp()
		{
			// TODO: create executorService with mock bus and real container
		}

		[Test ()]
		public void AddHandlers_should_discover_and_add_all_command_handlers ()
		{
		}
	}
}

