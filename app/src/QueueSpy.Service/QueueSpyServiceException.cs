using System;

namespace QueueSpy
{
	
	[Serializable]
	public class QueueSpyServiceException : Exception
	{
		public QueueSpyServiceException ()
		{
		}

		public QueueSpyServiceException (string format, params object[] args) : base (string.Format(format, args))
		{
		}

		public QueueSpyServiceException (string message) : base (message)
		{
		}

		public QueueSpyServiceException (string message, Exception inner) : base (message, inner)
		{
		}

		protected QueueSpyServiceException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
	}
}
