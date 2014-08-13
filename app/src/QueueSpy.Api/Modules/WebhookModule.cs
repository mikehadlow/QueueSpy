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

			Delete ["/{id}"] = parameters => DeleteWebhook (bus, parameters.id);
		}

		IEnumerable<Webhook> GetWebhooks (IDbReader dbReader, int userId)
		{
			return dbReader.Get<Webhook> ("UserId = :UserId", x => x.UserId = userId);
		}

		dynamic CreateNewWebhook (IBus bus, NewWebhookPost newWebhookPost)
		{
			if(string.IsNullOrWhiteSpace(newWebhookPost.url)) {
				return Respond.WithBadRequest ("You must enter a URL for the web hook.");
			}
			bus.SendCommand (new Messages.NewWebhook {
				Url = newWebhookPost.url,
				UserId = this.UserId()
			});

			return HttpStatusCode.OK;
		}

		dynamic DeleteWebhook (IBus bus, int id)
		{
			bus.SendCommand (new Messages.DeleteWebHook { WebHookId = id });

			return HttpStatusCode.OK;
		}
	}
}

