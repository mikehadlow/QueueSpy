using System.Linq;
using Nancy;

namespace QueueSpy.Api
{
	public class QueueModule : NancyModule
	{
		public QueueModule (IDbReader dbReader) : base("/queue")
		{
			Preconditions.CheckNotNull (dbReader, "dbReader");

			Get ["/{id}"] = parameters => GetQueueDetails (dbReader, parameters.id);

			Get ["/{id}/levels"] = parameters => GetQueueLevels (dbReader, parameters.id);
		}

		dynamic GetQueueDetails (IDbReader dbReader, int queueId)
		{
			try {
				var queue = dbReader.GetById<Queue> (queueId);
				var vhost = dbReader.GetById<VHost> (queue.VHostId);
				var broker = dbReader.GetById<Broker> (vhost.BrokerId);
				if(broker.UserId != this.UserId()) {
					return HttpStatusCode.NotFound;
				}
				return queue;
			} catch (RowNotFoundInTableException) {
				return HttpStatusCode.NotFound;
			}
		}

		dynamic GetQueueLevels (IDbReader dbReader, int queueId)
		{
			return dbReader.Get<QueueLevel> ("QueueId = :QueueId", x => x.QueueId = queueId).ToList();
		}
	}
}

