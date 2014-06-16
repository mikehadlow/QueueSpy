using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyNetQ;

namespace QueueSpy.Monitor
{
	public class MonitorService : IQueueSpyService
	{
		private readonly IBus bus;
		private readonly ILogger logger;
		private readonly IBrokerModelLoader brokerModelLoader;

		private readonly Compare<Broker, Messages.BrokerStatus, int> analysisModel;

		public MonitorService(IBus bus, ILogger logger, TinyIoC.TinyIoCContainer container, IBrokerModelLoader brokerModelLoader)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (logger, "logger");
			Preconditions.CheckNotNull (container, "container");
			Preconditions.CheckNotNull (brokerModelLoader, "brokerModelLoader");

			this.bus = bus;
			this.logger = logger;
			this.brokerModelLoader = brokerModelLoader;

			analysisModel = AnalysisModelBuilder.BuildAnalysisModel ();
		}

		public void Start()
		{
			bus.Subscribe<Messages.BrokerStatus> ("monitor", OnBrokerStatus);
		}

		void OnBrokerStatus (QueueSpy.Messages.BrokerStatus brokerStatus)
		{
			logger.Log ("BrokerStatus received, id: {0}.", brokerStatus.BrokerId);
			var brokerModel = brokerModelLoader.LoadBrokerModel (brokerStatus.BrokerId);

			var context = new CompareContext {
				Bus = bus
			};

			context.SendMessage = brokerEvent => {
				brokerEvent.BrokerId = brokerStatus.BrokerId;
				brokerEvent.DateTimeUTC = brokerStatus.SampledAtUtc;
				bus.SendCommand<Messages.BrokerEvent>(brokerEvent);
			};

			analysisModel.VisitUnchanged (brokerModel, brokerStatus, context);
		}
	}
}
