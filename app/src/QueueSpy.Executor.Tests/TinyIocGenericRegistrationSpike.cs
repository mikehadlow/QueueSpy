using NUnit.Framework;
using System;
using System.Reflection;
using TinyIoC;

namespace QueueSpy.Executor.Tests
{
	[TestFixture ()]
	public class TinyIocGenericRegistrationSpike
	{
		[Test ()]
		public void Should_be_able_to_register_and_resolve_generic_components ()
		{
			var container = TinyIoCContainer.Current;
			container.AutoRegister (t => t.Assembly == this.GetType ().Assembly);

			container.Register<IGenericThing<int>, GenericThing<int>> ();
			container.Register<IGenericThing<string>, StringThing> ();

			var thing = container.Resolve<IThing> ();
			thing.Do ();

			var genericThing = container.Resolve<IGenericThing<int>> ();
			var x = genericThing.GetIt ();
			Console.WriteLine ("Got {0} from GenericThing", x);

			var stringThing = container.Resolve<IGenericThing<string>> ();
			var s = stringThing.GetIt ();
			Console.WriteLine ("Got {0} from StringThing", s);

			container.Dispose ();
		}
	}

	public interface IThing
	{
		void Do();
	}

	public class Thing : IThing
	{
		public void Do()
		{
			Console.WriteLine("Did something!");
		}
	}

	public interface IGenericThing<T>
	{
		T GetIt();
	}

	public class GenericThing<T> : IGenericThing<T> where T : new()
	{
		public T GetIt()
		{
			return new T ();
		}
	}

	public class StringThing : IGenericThing<string>
	{
		public string GetIt ()
		{
			return "I am a thing!";
		}
	}
}

