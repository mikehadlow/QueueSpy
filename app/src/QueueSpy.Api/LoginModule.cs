using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ModelBinding;
using QueueSpy.Api.Authorization;

namespace QueueSpy.Api
{
	public class LoginModule : NancyModule
	{
		public LoginModule ()
		{
			Post ["/login/"] = _ => LoginHandler(this.Bind<LoginRequest>());
		}

		public JwtToken LoginHandler(LoginRequest loginRequest)
		{
			// TODO: validate user against the user database.

			var payload = new Dictionary<string, object> {
				{ "email", loginRequest.email },
				{ "userId", 101 }
			};

			var secretKey = System.Configuration.ConfigurationManager.AppSettings ["SecretKey"];
			var token = JsonWebToken.Encode (payload, secretKey, JwtHashAlgorithm.HS256);

			return new JwtToken { Token = token };
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

