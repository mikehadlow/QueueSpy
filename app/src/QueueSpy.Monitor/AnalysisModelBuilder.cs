using System;
using EasyNetQ;
using System.Linq;

namespace QueueSpy.Monitor
{
	public static class AnalysisModelBuilder
	{
		public static Compare<Broker, Messages.BrokerStatus, int> BuildAnalysisModel()
		{
			// Broker
			var brokerCompare = new Compare<Broker, Messages.BrokerStatus, int> {
				ModelKeySelector = m => m.Id,
				StatusKeySelector = m => m.BrokerId,
				OnUnchanged = CheckBrokerStatus
			};

			// VHost
			var vhostCompare = new Compare<VHost, Messages.VHost, string> {
				ModelKeySelector = m => m.Name,
				StatusKeySelector = s => s.Name,
				OnModelDeleted = OnVHostDeleted,
				OnNewStatus = OnVHostCreated
			};

			var vhostIterator = new ChildIterator<Broker, Messages.BrokerStatus, VHost, Messages.VHost, string> {
				ChildModelSelector = broker => broker.VHosts,
				ChildStatusSelector = brokerStatus => brokerStatus.VHosts,
				ChildCompare = vhostCompare
			};
			brokerCompare.AddChildIterator (vhostIterator);

			// Connection
			var connectionCompare = new Compare<Connection, Messages.Connection, string> {
				ModelKeySelector = m => m.Name,
				StatusKeySelector = s => s.Name,
				OnModelDeleted = OnConnectionLost,
				OnNewStatus = OnConnectionEstablished
			};

			var connectionIterator = new ChildIterator<VHost, Messages.VHost, Connection, Messages.Connection, string> {
				ChildModelSelector = vhost => vhost.Connections,
				ChildStatusSelector = vhost => vhost.Connections,
				ChildCompare = connectionCompare
			};
			vhostCompare.AddChildIterator (connectionIterator);

			// Consumer
			var consumerCompare = new Compare<Consumer, Messages.Consumer, string> {
				ModelKeySelector = m => m.Tag,
				StatusKeySelector = s => s.Tag,
				OnModelDeleted = OnConsumerCancel,
				OnNewStatus = OnNewConsumer
			};

			var consumerIterator = new ChildIterator<Connection, Messages.Connection, Consumer, Messages.Consumer, string> {
				ChildModelSelector = connection => connection.Consumers,
				ChildStatusSelector = connection => connection.Consumers,
				ChildCompare = consumerCompare
			};
			connectionCompare.AddChildIterator (consumerIterator);

			// Queue
			var queueCompare = new Compare<Queue, Messages.Queue, string> {
				ModelKeySelector = m => m.Name,
				StatusKeySelector = s => s.Name,
				OnModelDeleted = OnQueueDeleted,
				OnNewStatus = OnNewQueue,
				OnUnchanged = OnQueueLevel
			};

			var queueIterator = new ChildIterator<VHost, Messages.VHost, Queue, Messages.Queue, string> { 
				ChildModelSelector = vhost => vhost.Queues,
				ChildStatusSelector = vhost => vhost.Queues,
				ChildCompare = queueCompare
			};
			vhostCompare.AddChildIterator (queueIterator);

			return brokerCompare;
		}

		static void CheckBrokerStatus (Broker brokerModel, QueueSpy.Messages.BrokerStatus brokerStatus, CompareContext context)
		{
			if (!brokerStatus.IsResponding && brokerModel.ContactOK) {
				context.SendMessage(new Messages.BrokerEvent { 
					EventTypeId = (int)EventType.BrokerContactLost,
					Description = "Broker Contact Lost."
				});
			}

			if (brokerStatus.IsResponding && !brokerModel.ContactOK) {
				context.SendMessage(new Messages.BrokerEvent {
					EventTypeId = (int)EventType.BrokerContactEstablished,
					Description = "Broker Contact Established."
				});
			}
		}

		static void OnVHostDeleted (VHost vhost, CompareContext context)
		{
			context.SendMessage (new Messages.VHostDeleted {
				EventTypeId = (int)EventType.VHostDeleted,
				Description = string.Format("VHost {0} Deleted", vhost.Name),
				VHostId = vhost.Id
			});
		}

		static void OnVHostCreated (QueueSpy.Messages.VHost vhost, CompareContext context)
		{
			context.SendMessage (new Messages.VHostCreated {
				EventTypeId = (int)EventType.VHostCreated,
				Description = string.Format("VHost {0} Created", vhost.Name),
				Name = vhost.Name
			});
		}

		static void OnConnectionLost (Connection connection, CompareContext context)
		{
			context.SendMessage (new Messages.ConnectionLost {
				EventTypeId = (int)EventType.ConnectionLost,
				Description = string.Format ("Connection '{0}' Lost.", connection.Name),
				ConnectionId = connection.Id
			});
		}

		static void OnConnectionEstablished (QueueSpy.Messages.Connection connectionStatus, CompareContext context)
		{
			var vhost = context.GetStatusParent<Messages.VHost> ();
			context.SendMessage (new Messages.ConnectionEstablished { 
				EventTypeId = (int)EventType.ConnectionEstablished,
				Description = string.Format ("Connection '{0}' Established.", connectionStatus.Name),
				Name = connectionStatus.Name,
				VHostName = vhost.Name,
				Properties = connectionStatus.ClientProperties.ToDictionary (x => x.Key, x => x.Value)
			});
		}

		static void OnQueueDeleted (Queue queue, CompareContext context)
		{
			context.SendMessage (new Messages.QueueDeleted { 
				EventTypeId = (int)EventType.QueueDeleted,
				Description = string.Format ("Queue deleted: {0}", queue.Name),
				QueueId = queue.Id
			});
		}

		static void OnNewQueue (QueueSpy.Messages.Queue queueStatus, CompareContext context)
		{
			var vhost = context.GetStatusParent<Messages.VHost> ();
			context.SendMessage (new Messages.QueueCreated { 
				EventTypeId = (int)EventType.QueueCreated,
				Description = string.Format ("New queue created: {0}", queueStatus.Name),
				Name = queueStatus.Name,
				VHostName = vhost.Name
			});
		}

		static void OnQueueLevel (Queue queue, QueueSpy.Messages.Queue queueStatus, CompareContext context)
		{
			context.Bus.SendCommand (new Messages.QueueLevel {
				QueueId = queue.Id,
				Ready = queueStatus.Ready,
				Unacked = queueStatus.Unacked,
				Total = queueStatus.Total,
			});
		}

		static void OnConsumerCancel (Consumer consumer, CompareContext context)
		{
			context.SendMessage (new Messages.ConsumerCancelled { 
				EventTypeId = (int)EventType.ConsumerCancelled,
				Description = string.Format("Consumer stopped consuming from queue {0}", consumer.QueueName),
				ConsumerId = consumer.Id
			});
		}

		static void OnNewConsumer (QueueSpy.Messages.Consumer consumer, CompareContext context)
		{
			var connection = context.GetStatusParent<Messages.Connection> ();
			context.SendMessage (new Messages.NewConsumer { 
				EventTypeId = (int)EventType.NewConsumer,
				Description = string.Format("Consumer started consuming from queue {0}", consumer.QueueName),
				ConnectionName = connection.Name,
				Tag = consumer.Tag,
				QueueName = consumer.QueueName
			});
		}
	}
}

