using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace QueueSpy.Mailer.Tests
{
	[TestFixture ()]
	public class SesEmailSpike
	{
		private const string userName = "AKIAIVDEA4TOOYK2BIUA";
		private const string password = "AqtUmmC7OnnuVw3Y4rf4qwn74Y5AlsObtPb9TpJ0Utb+";

		[Test]
		public void Should_be_able_to_send_an_email_via_Amazon_SES_using_SMTP ()
		{
			ServicePointManager.ServerCertificateValidationCallback = (obj, cert, chain, errors) => true;
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

		[Test]
		public void Should_be_able_to_send_an_email_via_Amazon_SES_using_client_libs ()
		{
			var awsAccessKeyId = "";
			var awsSecretAccessKey = "";
			var client = new AmazonSimpleEmailServiceClient (awsAccessKeyId, awsSecretAccessKey, Amazon.RegionEndpoint.APNortheast1);

			var destination = new Destination {
				ToAddresses = new List<string> { "info@queuespy.com" }
			};
			var message = new Message (new Content("Hello from SES API!"), new Body(new Content("This is the body.")));
			var request = new SendEmailRequest ("info@queuespy.com", destination, message);

			client.SendEmail (request);
		}
	}
}

