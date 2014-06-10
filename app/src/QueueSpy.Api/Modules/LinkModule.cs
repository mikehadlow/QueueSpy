using System;
using Nancy;
using System.Collections.Generic;

namespace QueueSpy.Api
{
	/// <summary>
	/// Rather nasty UI concern in API. 
	/// Serves a tree of links to render in the console sidebar.
	/// </summary>
	public class LinkModule : NancyModule
	{
		public LinkModule (IBrokerService brokerService) : base("/links")
		{
			Get ["/"] = parameters => GetLinks (brokerService);
		}

		IEnumerable<QueueSpy.Api.Link> GetLinks (IBrokerService brokerService)
		{
			var links = new List<Api.Link> ();

			// hard coded first level links:
			var brokers = new Link { Href = "#/brokers", Label = "Brokers" };
			links.Add (brokers);
			links.Add (new Link { Href = "#/version", Label = "Version" });
			links.Add (new Link { Href = "#/users", Label = "Users" });
			links.Add (new Link { Href = "#/heartbeats", Label = "Heartbeats" });

			foreach(var broker in brokerService.GetUsersBrokers(this.UserId())) {
				var brokerLink = new Link { 
					Href = string.Format ("#/brokers/{0}", broker.Id),
					Label = broker.Url
				};

				brokers.Children.Add (brokerLink);

				foreach(var queue in brokerService.GetQueues(this.UserId(), broker.Id)) {
					brokerLink.Children.Add (new Link {
						Href = string.Format ("#/queues/{0}", queue.Id),
						Label = queue.Name
					});
				}
			}

			return links;
		}
	}

	public class Link
	{
		public string Href { get; set; }
		public string Label { get; set; }
		public IList<Link> Children { get; set; }

		public Link ()
		{
			Children = new List<Link> ();
		}
	}
}

