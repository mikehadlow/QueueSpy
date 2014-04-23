using NUnit.Framework;
using System;
using System.Collections.Generic;
using QueueSpy.Api.Authorization;

namespace QueueSpy.Api.Tests.Authorization
{
	[TestFixture]
	public class JwtTests
	{
		[Test]
		public void Should_be_able_to_generate_Jwt()
		{
			var payload = new Dictionary<string, object> {
				{ "user", "mike" },
				{ "exp", 123456789 }
			};

			var token = JsonWebToken.Encode (payload, TestAuthValues.SharedSecret, JwtHashAlgorithm.HS256);
			Console.WriteLine ("Token: '{0}'", token);
		}

		[Test]
		public void Should_be_able_to_verify_Jwt()
		{
			var jsonPayload = JsonWebToken.DecodeToObject (TestAuthValues.Token, TestAuthValues.SharedSecret) as Dictionary<string, object>;

			foreach (var item in jsonPayload) {
				Console.WriteLine ("{0}: {1}", item.Key, item.Value);
			}
		}
	}
}

