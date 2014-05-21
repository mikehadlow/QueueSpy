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

		private readonly IList<IAnalysisStep> analysisSteps = new List<IAnalysisStep> ();

		public MonitorService(IBus bus, ILogger logger, TinyIoC.TinyIoCContainer container)
		{
			Preconditions.CheckNotNull (bus, "bus");
			Preconditions.CheckNotNull (logger, "logger");
			Preconditions.CheckNotNull (container, "container");

			this.bus = bus;
			this.logger = logger;

			LoadAnalysisSteps (container);
		}

		void LoadAnalysisSteps (TinyIoC.TinyIoCContainer container)
		{
			var steps =
				from t in Assembly.GetCallingAssembly ().GetTypes ()
				where t.GetInterfaces ().Any (x => x.Name == typeof(IAnalysisStep).Name)
				select (IAnalysisStep)container.Resolve(t);
				
			foreach (var step in steps) {
				analysisSteps.Add (step);
				logger.Log ("Registered AnalysisStep: {0}", step.GetType ().Name);
			}
		}

		public void Start()
		{
			bus.Subscribe<Messages.BrokerStatus> ("monitor", OnBrokerStatus);
		}

		void OnBrokerStatus (QueueSpy.Messages.BrokerStatus brokerStatus)
		{
			logger.Log ("BrokerStatus received, id: {0}.", brokerStatus.BrokerId);
			foreach (var step in analysisSteps) {
				step.Analyse (brokerStatus);
			}
		}
	}

	/// <summary>
	/// Represents a step in the analysis of a broker status message.
	/// </summary>
	public interface IAnalysisStep
	{
		void Analyse(Messages.BrokerStatus brokerStatus);
	}
}
