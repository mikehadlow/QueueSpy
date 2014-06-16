using NUnit.Framework;
using System;
using System.Collections.Generic;
using Rhino.Mocks;

namespace QueueSpy.Monitor.Tests
{
	[TestFixture ()]
	public class AnalysisModelBuilderTests
	{
		private Compare<Broker, Messages.BrokerStatus, int> analyser;
		CompareContext context;

		private IList<EventType> brokerEventMessages;

		[SetUp]
		public void SetUp()
		{
			brokerEventMessages = new List<EventType> ();

			analyser = AnalysisModelBuilder.BuildAnalysisModel ();

			context = new CompareContext {
				Bus = MockRepository.GenerateStub<EasyNetQ.IBus>()
			};

			context.SendMessage = brokerEvent => {
				brokerEventMessages.Add((EventType)brokerEvent.EventTypeId);
				Console.WriteLine("Got {0}", (EventType)brokerEvent.EventTypeId);
				foreach(var property in brokerEvent.GetType().GetProperties()) {
					Console.WriteLine("{0}: {1}", property.Name, property.GetValue(brokerEvent));
				}
			};
		}

		[Test ()]
		public void Should_notice_broker_connected ()
		{
			var brokerModel = new Broker {
				Id = 101,
				Url = "http://mybroker/",
				Active = true,
				ContactOK = false
			};

			var brokerStatus = new Messages.BrokerStatus {
				BrokerId = 101,
				Url = "http://mybroker/",
				IsResponding = true
			};

			analyser.VisitUnchanged (brokerModel, brokerStatus, context);
			Assert.AreEqual (EventType.BrokerContactEstablished, brokerEventMessages [0]);
		}

		[Test]
		public void Should_notice_broker_contact_lost ()
		{
			var brokerModel = new Broker {
				Id = 101,
				Url = "http://mybroker/",
				Active = true,
				ContactOK = true
			};

			var brokerStatus = new Messages.BrokerStatus {
				BrokerId = 101,
				Url = "http://mybroker/",
				IsResponding = false
			};

			analyser.VisitUnchanged (brokerModel, brokerStatus, context);
			Assert.AreEqual (EventType.BrokerContactLost, brokerEventMessages [0]);
		}

		[Test]
		public void Should_notice_new_VHost ()
		{
			var brokerModel = new Broker {
				Id = 101,
				Url = "http://mybroker/",
				Active = true,
				ContactOK = true
			};

			var brokerStatus = new Messages.BrokerStatus {
				BrokerId = 101,
				Url = "http://mybroker/",
				IsResponding = true
			};

			brokerStatus.VHosts.Add (new Messages.VHost {
				Name = "/"
			});

			analyser.VisitUnchanged (brokerModel, brokerStatus, context);
			Assert.AreEqual (EventType.VHostCreated, brokerEventMessages [0]);
		}

		[Test]
		public void Should_notice_deleted_VHost ()
		{
			var brokerModel = new Broker {
				Id = 101,
				Url = "http://mybroker/",
				Active = true,
				ContactOK = true
			};

			var brokerStatus = new Messages.BrokerStatus {
				BrokerId = 101,
				Url = "http://mybroker/",
				IsResponding = true
			};

			brokerModel.VHosts.Add (new VHost {
				Id = 666,
				Name = "/"
			});

			analyser.VisitUnchanged (brokerModel, brokerStatus, context);
			Assert.AreEqual (EventType.VHostDeleted, brokerEventMessages [0]);
		}

		[Test]
		public void Should_notice_new_connection ()
		{
			var brokerModel = new Broker {
				Id = 101,
				Url = "http://mybroker/",
				Active = true,
				ContactOK = true
			};

			var brokerStatus = new Messages.BrokerStatus {
				BrokerId = 101,
				Url = "http://mybroker/",
				IsResponding = true
			};

			brokerModel.VHosts.Add (new VHost {
				Id = 666,
				Name = "/"
			});
			brokerStatus.VHosts.Add (new QueueSpy.Messages.VHost {
				Name = "/"
			});
			brokerStatus.VHosts [0].Connections.Add (new QueueSpy.Messages.Connection {
				Name = "localhost:12345 => localhost:54321"
			});

			analyser.VisitUnchanged (brokerModel, brokerStatus, context);
			Assert.AreEqual (EventType.ConnectionEstablished, brokerEventMessages [0]);
		}

		[Test]
		public void Should_notice_lost_connection ()
		{
			var brokerModel = new Broker {
				Id = 101,
				Url = "http://mybroker/",
				Active = true,
				ContactOK = true
			};

			var brokerStatus = new Messages.BrokerStatus {
				BrokerId = 101,
				Url = "http://mybroker/",
				IsResponding = true
			};

			brokerModel.VHosts.Add (new VHost {
				Id = 666,
				Name = "/"
			});
			brokerStatus.VHosts.Add (new QueueSpy.Messages.VHost {
				Name = "/"
			});
			brokerModel.VHosts [0].Connections.Add (new Connection {
				Id = 1024,
				Name = "my_connection"
			});

			analyser.VisitUnchanged (brokerModel, brokerStatus, context);
			Assert.AreEqual (EventType.ConnectionLost, brokerEventMessages [0]);
		}

		[Test]
		public void Should_notice_new_consumer ()
		{
			var brokerModel = new Broker {
				Id = 101,
				Url = "http://mybroker/",
				Active = true,
				ContactOK = true
			};

			var brokerStatus = new Messages.BrokerStatus {
				BrokerId = 101,
				Url = "http://mybroker/",
				IsResponding = true
			};

			brokerModel.VHosts.Add (new VHost {
				Id = 666,
				Name = "/"
			});
			brokerStatus.VHosts.Add (new QueueSpy.Messages.VHost {
				Name = "/"
			});

			brokerModel.VHosts [0].Connections.Add (new Connection {
				Id = 1024,
				Name = "my_connection"
			});
			brokerStatus.VHosts [0].Connections.Add (new QueueSpy.Messages.Connection {
				Name = "my_connection"
			});

			brokerStatus.VHosts [0].Connections [0].Consumers.Add (new QueueSpy.Messages.Consumer {
				Tag = "the.consumer.tag",
				QueueName = "the.queue.name"
			});

			analyser.VisitUnchanged (brokerModel, brokerStatus, context);
			Assert.AreEqual (EventType.NewConsumer, brokerEventMessages [0]);
		}

		[Test]
		public void Should_notice_cancelled_consumer ()
		{
			var brokerModel = new Broker {
				Id = 101,
				Url = "http://mybroker/",
				Active = true,
				ContactOK = true
			};

			var brokerStatus = new Messages.BrokerStatus {
				BrokerId = 101,
				Url = "http://mybroker/",
				IsResponding = true
			};

			brokerModel.VHosts.Add (new VHost {
				Id = 666,
				Name = "/"
			});
			brokerStatus.VHosts.Add (new QueueSpy.Messages.VHost {
				Name = "/"
			});

			brokerModel.VHosts [0].Connections.Add (new Connection {
				Id = 1024,
				Name = "my_connection"
			});
			brokerStatus.VHosts [0].Connections.Add (new QueueSpy.Messages.Connection {
				Name = "my_connection"
			});

			brokerModel.VHosts [0].Connections [0].Consumers.Add (new Consumer {
				Id = 444,
				Tag = "the.consumer.tag",
				QueueName = "the.queue.name"
			});

			analyser.VisitUnchanged (brokerModel, brokerStatus, context);
			Assert.AreEqual (EventType.ConsumerCancelled, brokerEventMessages [0]);
		}

		[Test]
		public void Should_notice_new_queue ()
		{
			var brokerModel = new Broker {
				Id = 101,
				Url = "http://mybroker/",
				Active = true,
				ContactOK = true
			};

			var brokerStatus = new Messages.BrokerStatus {
				BrokerId = 101,
				Url = "http://mybroker/",
				IsResponding = true
			};

			brokerModel.VHosts.Add (new VHost {
				Id = 666,
				Name = "/"
			});
			brokerStatus.VHosts.Add (new QueueSpy.Messages.VHost {
				Name = "/"
			});
			brokerStatus.VHosts [0].Queues.Add (new QueueSpy.Messages.Queue {
				Name = "My.Queue"
			});

			analyser.VisitUnchanged (brokerModel, brokerStatus, context);
			Assert.AreEqual (EventType.QueueCreated, brokerEventMessages [0]);

		}

		[Test]
		public void Should_notice_delted_queue ()
		{
			var brokerModel = new Broker {
				Id = 101,
				Url = "http://mybroker/",
				Active = true,
				ContactOK = true
			};

			var brokerStatus = new Messages.BrokerStatus {
				BrokerId = 101,
				Url = "http://mybroker/",
				IsResponding = true
			};

			brokerModel.VHosts.Add (new VHost {
				Id = 666,
				Name = "/"
			});
			brokerStatus.VHosts.Add (new QueueSpy.Messages.VHost {
				Name = "/"
			});
			brokerModel.VHosts [0].Queues.Add (new Queue {
				Id = 898,
				Name = "My.Queue"
			});

			analyser.VisitUnchanged (brokerModel, brokerStatus, context);
			Assert.AreEqual (EventType.QueueDeleted, brokerEventMessages [0]);
		}

		[Test]
		public void Should_notice_queue_level ()
		{
			var brokerModel = new Broker {
				Id = 101,
				Url = "http://mybroker/",
				Active = true,
				ContactOK = true
			};

			var brokerStatus = new Messages.BrokerStatus {
				BrokerId = 101,
				Url = "http://mybroker/",
				IsResponding = true
			};

			brokerModel.VHosts.Add (new VHost {
				Id = 666,
				Name = "/"
			});
			brokerModel.VHosts [0].Queues.Add (new Queue {
				Id = 898,
				Name = "My.Queue"
			});
			brokerStatus.VHosts.Add (new QueueSpy.Messages.VHost {
				Name = "/"
			});
			brokerStatus.VHosts [0].Queues.Add (new QueueSpy.Messages.Queue {
				Name = "My.Queue"
			});

			analyser.VisitUnchanged (brokerModel, brokerStatus, context);

			context.Bus.AssertWasCalled (x => x.Send (Arg<string>.Is.Anything, Arg<Messages.QueueLevel>.Is.Anything));
		}
	}
}

