﻿namespace QueueSpy.Executor
{
    class Program
    {
        static void Main(string[] args)
        {
			QueuespyApp.Run ();
        }
    }

	public class ExecutorService : IQueueSpyService
	{
		public void Start()
		{

		}
	}
}
