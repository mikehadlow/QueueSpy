using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using Nancy.Json;
using QueueSpy.Api.Authorization;
using Rhino.Mocks;

namespace QueueSpy.Api.Tests
{
	[TestFixture ()]
	public class UserModuleTests
	{
		private UserModule userModule;
		private IUserService userService;
		private EasyNetQ.IBus bus;

		[SetUp]
		public void SetUp()
		{
			userService = Rhino.Mocks.MockRepository.GenerateStub<IUserService> ();
			bus = Rhino.Mocks.MockRepository.GenerateStub<EasyNetQ.IBus> ();
			userModule = new UserModule (userService, bus);
		}

		[Test, Explicit("Integration test")]
		public void Should_be_able_to_register_a_new_user()
		{
			var uri = @"http://localhost:8080/user/";
			var postData = @"{ ""email"": ""yuna@suteki.co.uk"", ""password"": ""my_passw0rd!"" }";
			using (var client = new WebClient ()) {
				client.Headers [HttpRequestHeader.ContentType] = "application/json";
				client.Headers [HttpRequestHeader.Accept] = "application/json";

				try {
					client.UploadString (uri, postData);
				}
				catch (WebException e) {
					using (var reader = new StreamReader (e.Response.GetResponseStream ())) {
						Console.WriteLine (reader.ReadToEnd ());
					}
					throw;
				}
			}
		}

		[Test]
		public void Should_return_bad_request_if_no_user_is_given()
		{
			var response = userModule.RegisterUser (bus, userService, new RegisterUserPost { email = "", password = "" });
			Assert.That (response.StatusCode == Nancy.HttpStatusCode.BadRequest);
		}

		[Test]
		public void Should_return_bad_request_if_no_password_is_given()
		{
			var response = userModule.RegisterUser (bus, userService, new RegisterUserPost { email = "somebody@somewhere.com", password = "" });
			Assert.That (response.StatusCode == Nancy.HttpStatusCode.BadRequest);
		}

		[Test]
		public void Should_return_bad_request_if_email_is_duplicate()
		{
			userService.Stub (x => x.UserExists ("somebody@somewhere.com")).Return (true);
			var response = userModule.RegisterUser (bus, userService, new RegisterUserPost { email = "somebody@somewhere.com", password = "what_do_you_think?" });
			Assert.That (response.StatusCode == Nancy.HttpStatusCode.BadRequest);
		}

		[Test]
		public void Should_return_200_OK_on_success()
		{
			var response = userModule.RegisterUser (bus, userService, new RegisterUserPost { email = "somebody@somewhere.com", password = "what_do_you_think?" });
			Assert.That (response == Nancy.HttpStatusCode.Created);
		}
	}
}

