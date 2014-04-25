using NUnit.Framework;
using System;

namespace QueueSpy.Api.Tests
{
	[TestFixture ()]
	public class UserServiceTests
	{
		IUserService userService;

		[SetUp]
		public void SetUp()
		{
			userService = new UserService (new PasswordService (), new DbReader ());
		}

		[Test]
		public void IsValidUser_should_successfully_validate_user()
		{
			var isValid = userService.IsValidUser ("mike@suteki.co.uk", "my_passw0rd!");
			Assert.IsTrue (isValid);
		}

		[Test]
		public void GetUserByEmail_should_return_user()
		{
			var user = userService.GetUserByEmail ("mike@suteki.co.uk");
			Assert.IsNotNull (user);
		}

		[Test]
		public void GetAllUsers_should_return_all_users()
		{
			var users = userService.GetAllUsers ();
			Assert.IsNotNull (users);
			foreach (var user in users) {
				Console.WriteLine ("User: {0}", user.Email);
			}
		}
	}
}

