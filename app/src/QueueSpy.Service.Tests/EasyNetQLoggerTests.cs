using NUnit.Framework;
using System;

namespace QueueSpy.Service.Tests
{
	[TestFixture ()]
	public class EasyNetQLoggerTests
	{
		private EasyNetQLogger easyNetQLogger;

		[SetUp]
		public void SetUp()
		{
			easyNetQLogger = new EasyNetQLogger (new Logger());
		}

		[Test ()]
		public void Should_log_exceptions ()
		{
			easyNetQLogger.ErrorWrite (new Exception ("This is an exception!", new ApplicationException("This is an inner exception!")));
		}

		[Test]
		public void Should_log_errors ()
		{
			easyNetQLogger.ErrorWrite ("OMG! Something awfull has happened! {0} {1}", "first awful thing!", "second awful thing.");
		}

		[Test]
		public void Should_log_error_with_no_format ()
		{
			easyNetQLogger.ErrorWrite ("OMG!\nAnd some more {lines}\n!!!");
		}
	}
}

