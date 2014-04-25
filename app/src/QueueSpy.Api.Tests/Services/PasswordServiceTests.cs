using NUnit.Framework;
using System;

namespace QueueSpy.Api.Tests
{
	[TestFixture ()]
	public class PasswordServiceTests
	{
		private IPasswordService passwordService;

		[SetUp]
		public void SetUp()
		{
			passwordService = new PasswordService ();
		}

		[Test]
		public void Should_be_able_to_hash_password()
		{
			const string password = "my_passw0rd!";
			var hashResult = passwordService.Hash (password);
			Console.WriteLine ("Hash: '{0}'", hashResult.PasswordHash);
			Console.WriteLine ("Salt: '{0}'", hashResult.Salt);

			var isValid = passwordService.PasswordValidates (password, hashResult.PasswordHash, hashResult.Salt);
			Assert.IsTrue (isValid);
		}
	}
}

