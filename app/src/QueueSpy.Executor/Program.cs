namespace QueueSpy.Executor
{
    class Program
    {
        static void Main(string[] args)
        {
			// load the QueueSpy assembly into the app domain.
			var name = typeof(QueueSpy.IPasswordService).Name;

			QueuespyApp.Run ();
		}
    }
}
