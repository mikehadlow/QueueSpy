using EasyNetQ;
using System;
using System.Threading;

namespace QueueSpy.Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
			QueuespyApp.Run (bus => {});
        }
    }
}
