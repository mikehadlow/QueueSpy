using System;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using EasyNetQ;

namespace QueueSpy.Api
{
	
	[Serializable]
	public class BrokerNotFoundException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:BrokerNotFoundException"/> class
		/// </summary>
		public BrokerNotFoundException ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BrokerNotFoundException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public BrokerNotFoundException (string message) : base (message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BrokerNotFoundException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public BrokerNotFoundException (string message, Exception inner) : base (message, inner)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BrokerNotFoundException"/> class
		/// </summary>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected BrokerNotFoundException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
	}
}
