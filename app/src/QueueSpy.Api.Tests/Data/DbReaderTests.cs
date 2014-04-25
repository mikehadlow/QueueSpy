using NUnit.Framework;

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
	}
}

