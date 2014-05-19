namespace QueueSpy
{
	public enum EventType
	{
		BrokerContactEstablished = 1,
		BrokerContactLost = 2,
		ConnectionEstablished = 3, 	// a client connection to the broker is established.
		ConnectionLost = 4			// a client connection is lost.
	}
}

