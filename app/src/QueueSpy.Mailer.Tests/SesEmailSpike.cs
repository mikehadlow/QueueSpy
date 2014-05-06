using NUnit.Framework;
using System;
using System.Net.Mail;
using System.Net;

namespace QueueSpy.Mailer.Tests
{
	[TestFixture ()]
	public class SesEmailSpike
	{
		private const string userName = "AKIAIYVDQNDVBIHKBVGQ";
		private const string password = "AuGMKBrcFge0JPpJuIrmZQyoU70EPSsl2VDdlOWxbhGq";

		[Test]
		public void Should_be_able_to_send_an_email_via_Amazon_SES_using_SMTP ()
		{
			using (var smtp = new SmtpClient {
				Host = "email-smtp.us-east-1.amazonaws.com",
				Port = 587,
				EnableSsl = true,
				Credentials = new NetworkCredential (userName, password)
			}) {
				using (var message = new MailMessage ("info@queuespy.com", "info@queuespy.com") {
					Subject = "Hello from the unit test!",
					Body = "And this is the message body. It's not HTML (yet)."
				}) {
					smtp.Send (message);
				}
			}
		}
	}
}

