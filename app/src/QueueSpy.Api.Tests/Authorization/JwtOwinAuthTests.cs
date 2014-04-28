using NUnit.Framework;
using System;
using System.Net;

namespace QueueSpy.Api.Tests.Authorization
{
	[TestFixture ()]
	public class JwtOwinAuthTests
	{
		private const string baseUri = @"http://localhost:8080/";
		private const string versionUri = baseUri + @"version/";

		[Test]
		[ExpectedException(typeof(WebException))]
		public void Unauthorized_Api_request_should_be_rejected()
		{
			using (var client = new WebClient ()) {
				client.Headers [HttpRequestHeader.Accept] = "application/json";

				client.DownloadString (versionUri);
			}
		}

		[Test]
		public void Authorized_Api_Request_should_be_accepted()
		{
			using (var client = new WebClient ()) {
				client.Headers [HttpRequestHeader.Accept] = "application/json";
				client.Headers [HttpRequestHeader.Authorization] = "Bearer " + TestAuthValues.Token;

				var result = client.DownloadString (versionUri);
				Console.WriteLine ("version result: '{0}'", result);
			}
		}

		[Test]
		public void Unauthorized_base_Uri_request_should_be_accepted()
		{
			using (var client = new WebClient ()) {
				client.Headers [HttpRequestHeader.Accept] = "application/json";

				client.DownloadString (baseUri);
			}
		}
	}
}

