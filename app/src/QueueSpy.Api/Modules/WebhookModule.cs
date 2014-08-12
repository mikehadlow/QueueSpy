using System;
using Nancy;
using Nancy.ModelBinding;
using EasyNetQ;
using System.Collections.Generic;

namespace QueueSpy.Api
{
	public class WebhookModule : NancyModule
	{
		public WebhookModule (IBus bus, IDbReader dbReader) : base("/webhook")
		{
			if (dbReader == null) throw new ArgumentNullException ("dbReader");

			Get ["/"] = parameters => GetWebhooks (dbReader, this.UserId());

			Post ["/"] = _ => CreateNewWebhook (bus, this.Bind<NewWebhookPost>());
		}

		IEnumerable<Webhook> GetWebhooks (IDbReader dbReader, int userId)
		{
			return dbReader.Get<Webhook> ("UserId = :UserId", x => x.UserId = userId);
		}

		object CreateNewWebhook (IBus bus, NewWebhookPost newWebhookPost)
		{
			bus.SendCommand (new Messages.NewWebhook {
				Url = newWebhookPost.url,
				UserId = newWebhookPost.userId
			});

			return HttpStatusCode.OK;
		}
	}
}

