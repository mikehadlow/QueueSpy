using EasyNetQ;

namespace QueueSpy.Alerter
{

	public class CommandAlertSink : IAlertSink
	{
		private readonly IBus bus;

		public CommandAlertSink(IBus bus)
		{
			Preconditions.CheckNotNull (bus, "bus");
			this.bus = bus;
		}

		public void Handle (AlertInfo alertInfo)
		{
			bus.SendCommand<Messages.Alert> (new Messages.Alert {
				BrokerId = alertInfo.Broker.Id,
				AlertTypeId = (int)alertInfo.AlertType,
				DateTimeUTC = alertInfo.DateTimeUTC,
				Description = alertInfo.Description
			});
		}
	}
}
