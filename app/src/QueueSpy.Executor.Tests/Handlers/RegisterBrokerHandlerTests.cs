using NUnit.Framework;
using System;

namespace QueueSpy.Executor.Tests
{
	[TestFixture ()]
	public class RegisterBrokerHandlerTests
	{
		private RegisterBrokerHandler handler;

		[SetUp]
		public void SetUp()
		{
			handler = new RegisterBrokerHandler (new DataWriter ());
		}

		[Test ()]
		public void Should_insert_a_new_broker_into_the_database ()
		{
			var registerBroker = new Messages.RegisterBroker {
				UserId = 1,
				Url = "http://my.broker.com/",
				Username = "mike",
				Password = "my_password"
			};

			handler.Handle (registerBroker);
		}
	}
}

