using System;

namespace QueueSpy
{
	public interface IDateService
	{
		long NowUnixTime { get; }
		DateTime DateTimeNow { get; }
		DateTime ToDateTime(long unixTime);
		long FromDateTime(DateTime dateTime);
		long UnixTimeIn(TimeSpan timeSpan);
		bool HasExpired(long unixTime);
	}

	public class DateService : IDateService
	{
		private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	    public long FromDateTime(DateTime dateTime)
	    {
	        if (dateTime.Kind == DateTimeKind.Utc)
	        {
	            return (long)dateTime.Subtract(epoch).TotalSeconds;
	        }
	        throw new ArgumentException("Input DateTime must be UTC");
	    }

	    public DateTime ToDateTime(long unixTime)
	    {
	        return epoch.AddSeconds(unixTime);
	    }

	    public long NowUnixTime {
	        get {
	            return FromDateTime(DateTime.UtcNow);
	        }
	    }

		public DateTime DateTimeNow {
			get {
				return DateTime.UtcNow;
			}
		}

		public long UnixTimeIn(TimeSpan timeSpan)
		{
			return FromDateTime (DateTimeNow.Add (timeSpan));
		}

		public bool HasExpired (long unixTime)
		{
			return unixTime < NowUnixTime;
		}
	}
}

