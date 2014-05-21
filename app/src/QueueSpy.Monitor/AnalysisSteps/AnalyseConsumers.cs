using System;
using System.Linq;
using EasyNetQ;
using System.Collections.Generic;

namespace QueueSpy.Monitor
{
	public class AnalyseConsumers : IAnalysisStep
	{
		private readonly IBus bus;
		private readonly IDbReader dbReader;
		private readonly ILogger logger;

		public AnalyseConsumers (IBus bus, IDbReader dbReader, ILogger logger)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (dbReader, "dbReader");
			Preconditions.CheckNotNull (logger, "logger");

			this.bus = bus;
			this.dbReader = dbReader;
			this.logger = logger;
		}

		public IEnumerable<QueueSpy.Consumer> LoadConsumers(int brokerId)
		{
			return dbReader.Get<QueueSpy.Consumer, QueueSpy.Connection> ("BrokerId = :BrokerId AND IsConsuming = TRUE", x => x.BrokerId = brokerId);
		}

		public void Analyse (QueueSpy.Messages.BrokerStatus brokerStatus)
		{
			var existingConsumers = LoadConsumers (brokerStatus.BrokerId);
			var existingTags = existingConsumers.Select(x => x.Tag);
			var retrievedTags = brokerStatus.Connections.SelectMany (x => x.Consumers).Select (x => x.Tag);

			// find closed consumers
			var closedTags = existingTags.Except (retrievedTags);
			var closedConsumers = existingConsumers.Where (x => closedTags.Any (t => t == x.Tag));
			foreach (var closedConsumer in closedConsumers) {
				bus.SendCommand (new Messages.ConsumerCancelled { 
					BrokerId = brokerStatus.BrokerId,
					EventTypeId = (int)EventType.ConsumerCancelled,
					DateTimeUTC = DateTime.UtcNow,
					Description = string.Format("Consumer started consuming from queue {0}", closedConsumer.QueueName),
					ConsumerId = closedConsumer.Id
				});
			}

			// find new consumers
			var newTags = retrievedTags.Except (existingTags);
			foreach (var connection in brokerStatus.Connections) {
				foreach (var consumer in connection.Consumers) {
					if (newTags.Any (x => x == consumer.Tag)) {
						bus.SendCommand (new Messages.NewConsumer { 
							BrokerId = brokerStatus.BrokerId,
							EventTypeId = (int)EventType.NewConsumer,
							DateTimeUTC = DateTime.UtcNow,
							Description = string.Format("Consumer cancelled consuming from queue {0}", consumer.QueueName),
							ConnectionName = connection.Name,
							Tag = consumer.Tag,
							QueueName = consumer.QueueName
						});
					}
				}
			}
		}
	}
}

