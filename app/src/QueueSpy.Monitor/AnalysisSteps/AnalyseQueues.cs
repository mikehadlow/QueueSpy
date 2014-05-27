﻿using System;
using System.Linq;
using EasyNetQ;

namespace QueueSpy.Monitor
{
	public class AnalyseQueues : IAnalysisStep
	{
		private readonly IDbReader dbReader;
		private readonly IBus bus;

		public AnalyseQueues (IDbReader dbReader, IBus bus)
		{
			Preconditions.CheckNotNull (dbReader, "dbReader");
			Preconditions.CheckNotNull (bus, "bus");

			this.dbReader = dbReader;
			this.bus = bus;
		}

		public void Analyse (QueueSpy.Messages.BrokerStatus brokerStatus)
		{
			if (brokerStatus.IsResponding) {
				var queues = dbReader.Get<Queue, Broker> ("BrokerId = :BrokerId AND IsCurrent = TRUE", x => x.BrokerId = brokerStatus.BrokerId);

				var retrievedQueues = brokerStatus.Queues.Select (x => x.Name);
				var existingQueues = queues.Select (x => x.Name);

				// find new queues
				var newQueues = retrievedQueues.Except (existingQueues);
				foreach (var newQueue in newQueues) {
					bus.SendCommand (new Messages.QueueCreated { 
						BrokerId = brokerStatus.BrokerId,
						EventTypeId = (int)EventType.QueueCreated,
						DateTimeUTC = brokerStatus.SampledAtUtc,
						Description = string.Format ("New queue created: {0}", newQueue),
						Name = newQueue
					});
				}

				// find deleted queues
				var deletedQueues = existingQueues.Except (retrievedQueues);
				foreach (var deletedQueue in deletedQueues) {
					bus.SendCommand (new Messages.QueueDeleted { 
						BrokerId = brokerStatus.BrokerId,
						EventTypeId = (int)EventType.QueueDeleted,
						DateTimeUTC = brokerStatus.SampledAtUtc,
						Description = string.Format ("Queue deleted: {0}", deletedQueue),
						QueueId = queues.Single (x => x.Name == deletedQueue).Id
					});
				}

				// publish queue levels
				foreach (var retrievedQueue in brokerStatus.Queues) {
					var queue = queues.SingleOrDefault (x => x.Name == retrievedQueue.Name);
					if(queue != null) {
						bus.SendCommand (new Messages.QueueLevel {
							QueueId = queue.Id,
							Ready = retrievedQueue.Ready,
							Unacked = retrievedQueue.Unacked,
							Total = retrievedQueue.Total,
							SampledAt = brokerStatus.SampledAtUtc
						});
					}
				}
			}
		}
	}
}

