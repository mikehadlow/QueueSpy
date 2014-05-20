using System;
using EasyNetQ;
using System.Threading;
using TinyIoC;

namespace QueueSpy
{

	public class EasyNetQLogger : IEasyNetQLogger
	{
		private readonly ILogger logger;

		public EasyNetQLogger(ILogger logger)
		{
			this.logger = logger;
		}

		public void DebugWrite (string format, params object[] args)
		{
			// no debug
		}

		public void InfoWrite (string format, params object[] args)
		{
			logger.Log ("[EasyNetQ-INFO] " + format, args);
		}

		public void ErrorWrite (string format, params object[] args)
		{
			logger.Log ("[EasyNetQ-ERROR] " + format, args);
		}

		public void ErrorWrite (Exception exception)
		{
			logger.Log ("[EasyNetQ-ERROR] {0}", exception.ToString());
		}
	}
}
