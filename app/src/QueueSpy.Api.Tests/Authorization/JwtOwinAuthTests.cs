using NUnit.Framework;
using System;
using System.Net;
using QueueSpy.Api.Authorization;
using Nancy.Json;

namespace QueueSpy.Api.Tests.Authorization
{
	[TestFixture ()]
	public class JwtOwinAuthTests
	{
		private readonly JavaScriptSerializer jsonSerializer = new JavaScriptSerializer { RetainCasing = true };
		private const string uri = @"http://localhost:8080/version/";
		private const string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoibWlrZSIsImV4cCI6MTIzNDU2Nzg5fQ.KG-ds05HT7kK8uGZcRemhnw3er_9brQSF1yB2xAwc_E";

		[Test]
		[ExpectedException(typeof(WebException))]
		public void Unauthorized_Api_request_should_be_rejected()
		{
			using (var client = new WebClient ()) {
				client.Headers [HttpRequestHeader.Accept] = "application/json";

				client.DownloadString (uri);
			}
		}

		[Test]
		public void Authorized_Api_Request_should_be_accepted()
		{
			using (var client = new WebClient ()) {
				client.Headers [HttpRequestHeader.Accept] = "application/json";
				client.Headers [HttpRequestHeader.Authorization] = "Bearer " + token;

				var result = client.DownloadString (uri);
				Console.WriteLine ("version result: '{0}'", result);
			}
		}
	}
}

