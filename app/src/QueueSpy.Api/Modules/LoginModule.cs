using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ModelBinding;
using QueueSpy.Authorization;

namespace QueueSpy.Api
{
	public class LoginModule : NancyModule
	{
		private readonly string secretKey;
		private readonly IUserService userService;

		public LoginModule (IUserService userService)
		{
			Preconditions.CheckNotNull (userService, "userService");
			this.userService = userService;

			Post ["/login/"] = _ => LoginHandler(this.Bind<LoginRequest>());

			secretKey = System.Configuration.ConfigurationManager.AppSettings ["SecretKey"];
		}

		public dynamic LoginHandler(LoginRequest loginRequest)
		{
			if (userService.IsValidUser (loginRequest.email, loginRequest.password)) {

				var user = userService.GetUserByEmail (loginRequest.email);

				var payload = new Dictionary<string, object> {
					{ "email", user.Email },
					{ "userId", user.Id }
				};

				var token = JsonWebToken.Encode (payload, secretKey, JwtHashAlgorithm.HS256);

				return new JwtToken { Token = token };
			} else {
				return HttpStatusCode.Unauthorized;
			}
		}
	}

	public class JwtToken
	{
		public string Token { get; set; }
	}

	public class LoginRequest
	{
		public string email { get; set; }
		public string password { get; set; }
	}
}

