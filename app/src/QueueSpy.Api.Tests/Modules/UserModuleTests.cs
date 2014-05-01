using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using Nancy.Json;
using QueueSpy.Api.Authorization;

namespace QueueSpy.Api.Tests
{
	[TestFixture ()]
	public class UserModuleTests
	{
		private readonly JavaScriptSerializer jsonSerializer = new JavaScriptSerializer { RetainCasing = true };

		[Test]
		public void Should_be_able_to_get_JWT_from_LoginHandler()
		{
			var uri = @"http://localhost:8080/user/";
			var postData = @"{ ""email"": ""yuna@suteki.co.uk"", ""password"": ""my_passw0rd!"" }";
			using (var client = new WebClient ()) {
				client.Headers [HttpRequestHeader.ContentType] = "application/json";
				client.Headers [HttpRequestHeader.Accept] = "application/json";

				try {
					var result = client.UploadString (uri, postData);
				}
				catch (WebException e) {
					using (var reader = new StreamReader (e.Response.GetResponseStream ())) {
						Console.WriteLine (reader.ReadToEnd ());
					}
					throw;
				}
			}
		}
	}
}

