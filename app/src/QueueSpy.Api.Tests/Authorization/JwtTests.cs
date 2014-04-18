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

			var secretKey = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
			var token = JsonWebToken.Encode (payload, secretKey, JwtHashAlgorithm.HS256);
			Console.WriteLine ("Token: '{0}'", token);
		}

		[Test]
		public void Should_be_able_to_verify_Jwt()
		{
			var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoibWlrZSIsImV4cCI6MTIzNDU2Nzg5fQ.KG-ds05HT7kK8uGZcRemhnw3er_9brQSF1yB2xAwc_E";
			var secretKey = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";

			var jsonPayload = JsonWebToken.DecodeToObject (token, secretKey) as Dictionary<string, object>;

			foreach (var item in jsonPayload) {
				Console.WriteLine ("{0}: {1}", item.Key, item.Value);
			}
		}
	}
}

