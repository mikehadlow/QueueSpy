using System;
using NUnit.Framework;

namespace QueueSpy.Executor.Tests
{
	[TestFixture]
	public class DataWriterTests
	{
		private IDataWriter dataWriter;

		[SetUp]
		public void SetUp()
		{
			dataWriter = new DataWriter ();
		}

		[Test]
		public void Should_be_able_to_insert_a_user ()
		{
			var user = new User {
				Email = "john@suteki.co.uk",
				PasswordHash = "blah",
				Salt = "foo"
			};

			var id = dataWriter.Insert (user);
			Console.WriteLine (id);

			dataWriter.Delete<User> (id);
		}
	}
}

