using System;

namespace QueueSpy.Executor
{
	public class WebhookHandler : ICommandHandler<Messages.NewWebhook>
	{
		private readonly IDataWriter dataWriter;

		public WebhookHandler (IDataWriter dataWriter)
		{
			Preconditions.CheckNotNull (dataWriter, "dataWriter");

			this.dataWriter = dataWriter;
		}

		public void Handle (QueueSpy.Messages.NewWebhook command)
		{
			dataWriter.Insert (new Executor.Webhook {
				Url = command.Url,
				UserId = command.UserId
			});
		}
	}
}

