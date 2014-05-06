using System;
using System.Net;
using System.Net.Mail;
using EasyNetQ;

namespace QueueSpy.Mailer
{

	public class MailerService : IQueueSpyService, IDisposable
	{
		private readonly IBus bus;
		private SmtpClient smtpClient;

		private const string userName = "AKIAIVDEA4TOOYK2BIUA";
		private const string password = "AqtUmmC7OnnuVw3Y4rf4qwn74Y5AlsObtPb9TpJ0Utb+";
		private const string fromAddress = "info@queuespy.com";

		public MailerService(IBus bus)
		{
			this.bus = bus;
		}

		public void Start()
		{
			// hack because mono can't seem to look up TLS cert chains?
			ServicePointManager.ServerCertificateValidationCallback = (obj, cert, chain, errors) => true;

			smtpClient = new SmtpClient {
				Host = "email-smtp.us-east-1.amazonaws.com",
				Port = 587,
				EnableSsl = true,
				Credentials = new NetworkCredential (userName, password)
			};

			bus.Subscribe<Messages.SendEmailRequest> ("mailer", HandleEmailRequest);
		}

		public void HandleEmailRequest(Messages.SendEmailRequest emailRequest)
		{
			using (var message = new MailMessage (fromAddress, emailRequest.ToAddress) {
				Subject = emailRequest.Subject,
				Body = emailRequest.Body
			}) {
				smtpClient.Send (message);
			}
		}

		public void Dispose ()
		{
			smtpClient.Dispose ();
		}
	}
}
