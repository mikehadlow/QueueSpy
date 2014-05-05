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

		[Test]
		public void Should_be_able_to_update_a_user ()
		{
			var user = new User {
				Email = "yuna@suteki.co.uk",
				PasswordHash = "biff",
				Salt = "foo"
			};

			var id = dataWriter.Insert (user);

			dataWriter.Update<User> (id, x => {
				x.PasswordHash = "updated_pw_hash";
				x.Salt = "updated_salt";
			});

			dataWriter.Delete<User> (id);
		}
	}
}

