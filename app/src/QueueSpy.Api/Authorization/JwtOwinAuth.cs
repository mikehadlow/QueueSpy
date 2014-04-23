using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueSpy.Api.Authorization
{
	using AppFunc = Func<IDictionary<string, object>, Task>;

	public class JwtOwinAuth
	{
		private readonly AppFunc next;
		private readonly string secretKey;

		public JwtOwinAuth (AppFunc next)
		{
			this.next = next;
			secretKey = System.Configuration.ConfigurationManager.AppSettings ["SecretKey"];
		}

		public Task Invoke(IDictionary<string, object> environment)
		{
			var path = environment ["owin.RequestPath"] as string;
			if (path == null) {
				throw new ApplicationException ("Invalid OWIN request. Expected owin.RequestPath, but not present.");
			}
			if (!path.StartsWith("/login")) {
				var headers = environment ["owin.RequestHeaders"] as IDictionary<string, string[]>;
				if (headers == null) {
					throw new ApplicationException ("Invalid OWIN request. Expected owin.RequestHeaders to be an IDictionary<string, string[]>.");
				}
				if (headers.ContainsKey ("Authorization")) {
					var token = GetTokenFromAuthorizationHeader (headers ["Authorization"]);
					try {
						var payload = JsonWebToken.DecodeToObject (token, secretKey);
						// TODO: set current user using payload data.
					} catch (SignatureVerificationException) {
						return UnauthorizedResponse (environment);
					}
				} else {
					return UnauthorizedResponse (environment);
				}
			}
			return next (environment);
		}

		public string GetTokenFromAuthorizationHeader(string[] authorizationHeader)
		{
			if (authorizationHeader.Length == 0) {
				throw new ApplicationException ("Invalid authorization header. It must have at least one element");
			}
			var token = authorizationHeader [0].Split (' ') [1];
			return token;
		}

		public Task UnauthorizedResponse(IDictionary<string, object> environment)
		{
			environment ["owin.ResponseStatusCode"] = 401;
			return Task.FromResult (0);
		}
	}
}

