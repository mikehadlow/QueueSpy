using System;

namespace QueueSpy.Alerter
{
	public class AlerterService : IQueueSpyService
	{
		public AlerterService ()
		{
		}

		public void Start ()
		{
			Console.WriteLine ("Alerter Service Started.");
		}
	}
}

