using NUnit.Framework;
using System.Collections.Generic;

namespace QueueSpy.Api.Tests
{
	[TestFixture ()]
	public class DbReaderTests
	{
		private IDbReader dbReader;

		[SetUp]
		public void SetUp()
		{
			dbReader = new DbReader ();
		}

		[Test]
		public void Should_be_able_to_get_User_from_database()
		{
			var user = dbReader.GetById<User> (1);
			Assert.IsNotNull (user);
			Assert.AreEqual (user.Email, "mike@suteki.co.uk");
		}

		[Test]
		public void Should_be_able_to_do_a_table_join()
		{
			var consumers = dbReader.Get<Consumer, Connection> ("BrokerId = :BrokerId", x => x.BrokerId = 4);
			foreach (var consumer in consumers) {
				System.Console.WriteLine (consumer.QueueName);
			}
		}

		[Test]
		public void Should_be_able_to_detect_collection_types()
		{
			Assert.IsTrue (DbReader.IsCollectionType (typeof(List<string>)));
			Assert.IsTrue (DbReader.IsCollectionType (typeof(Dictionary<string, string>)));
			Assert.IsFalse (DbReader.IsCollectionType (typeof(string)));
		}
	}
}

