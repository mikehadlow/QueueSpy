using System;

namespace QueueSpy.Executor
{
	public enum AlertType
	{
		BrokerContactEstablished = 1,
		BrokerContactLost = 2,
		ConnectionEstablished = 3, 	// a client connection to the broker is established.
		ConnectionLost = 4,			// a client connection is lost.
		ConsumerCancelled = 5,
		NewConsumer = 6,
		QueueCreated = 7,
		QueueDeleted = 8,
		VHostDeleted = 9,
		VHostCreated = 10
	}
}

