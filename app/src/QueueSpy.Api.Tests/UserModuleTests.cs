using NUnit.Framework;
using System;
using System.Linq;
using Nancy;

namespace QueueSpy.Api.Tests
{
	[TestFixture ()]
	public class UserModuleTests
	{
		private UserModule userModule;

		[SetUp]
		public void SetUp()
		{
			userModule = new UserModule ();
		}

		[Test]
		public void GetAllUsers_ShouldReturnAllUsers()
		{
			var users = userModule.GetAllUsers ();

			Assert.AreEqual (3, users.Count ());
		}
	}
}

