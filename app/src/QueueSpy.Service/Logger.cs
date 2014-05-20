using System;

namespace QueueSpy
{
	public interface ILogger
	{
		void Log (string message);

		void Log (string format, params object[] args);
	}

	public class Logger : ILogger
	{
		public void Log (string message)
		{
			Console.WriteLine ("[{0}] {1}", DateTime.UtcNow.ToString("O"), message);
		}

		public void Log (string format, params object[] args)
		{
			Log(SafeFormat(format, args));
		}

		public string SafeFormat(string format, params object[] args)
        {
			return args.Length == 0 ? format : string.Format (format, args);
        }
	}
}

