using System;
using System.Linq;
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
			var userId = this.UserId ();

			// hard coded first level links:
			var brokers = new Link { Href = "#/brokers", Label = "Brokers" };
			links.Add (brokers);

			links.Add (new Link { Href = "#/webhooks", Label = "Web Hooks" });
			links.Add (new Link { Href = "#/version", Label = "Version" });
			links.Add (new Link { Href = "#/users", Label = "Users" });
			links.Add (new Link { Href = "#/heartbeats", Label = "Heartbeats" });

			foreach(var broker in brokerService.GetUsersBrokers(userId)) {
				var brokerLink = new Link { 
					Href = string.Format ("#/brokers/{0}", broker.Id),
					Label = broker.Url
				};
				brokers.Children.Add (brokerLink);

				brokerLink.Children.Add (new Link { 
					Href = string.Format("#/events/{0}", broker.Id),
					Label = "Events"
				});

				brokerLink.Children.Add (new Link { 
					Href = string.Format("#/alerts/{0}", broker.Id),
					Label = "Alerts"
				});

				brokerLink.Children.Add (new Link { 
					Href = string.Format("#/connections/{0}", broker.Id),
					Label = "Connections"
				});

				var vhostsLink = new Link {
					Href = string.Format("#/unknown"),
					Label = "VHosts"
				};
				brokerLink.Children.Add (vhostsLink);

				var queues = brokerService.GetQueues (userId, broker.Id);
				var vhosts = brokerService.GetVHosts (userId, broker.Id);

				foreach (var vhost in vhosts) {
					var vhostLink = new Link {
						Href = string.Format ("#/vhosts/{0}", vhost.Id),
						Label = vhost.Name
					};
					vhostsLink.Children.Add (vhostLink);

					foreach (var queue in queues.Where(x => x.VHostId == vhost.Id)) {
						vhostLink.Children.Add (new Link {
							Href = string.Format ("#/queues/{0}", queue.Id),
							Label = queue.Name
						});
					}
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

