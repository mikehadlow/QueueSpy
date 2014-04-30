using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Linq;
using EasyNetQ;
using EasyNetQ.Consumer;

namespace QueueSpy.Executor.Tests
{
	[TestFixture ()]
	public class ExecutorServiceTests
	{
		ExecutorService executorService;

		[SetUp]
		public void SetUp()
		{
			var bus = MockRepository.GenerateStub<IBus> ();
			var container = TinyIoC.TinyIoCContainer.Current;

			executorService = new ExecutorService (bus, container);
		}

		[Test ()]
		public void AddHandlers_should_discover_and_add_all_command_handlers ()
		{
			var receiveRegistration = MockRepository.GenerateStub<IReceiveRegistration> ();

			executorService.AddHandlers (receiveRegistration);

			receiveRegistration.AssertWasCalled (x => x.Add<string> (Arg<Action<string>>.Is.Anything));
		}

		[Test]
		public void ReflectionSpike()
		{
			var implementsICommandHandler = typeof(TestHandler).GetInterfaces ().Any (x => x.Name == "ICommandHandler`1");
			Assert.IsTrue (implementsICommandHandler);
		}

		[Test]
		public void MethodNameSpike()
		{
			foreach (var method in typeof(ExecutorService).GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)) {
				Console.WriteLine (method.Name);
			}
		}
	}
}
