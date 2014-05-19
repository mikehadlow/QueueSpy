using System;
using System.Linq;
using System.Collections.Generic;

namespace QueueSpy.Service
{
	public static class TinyIocExtensions
	{
		public static IEnumerable<T> ResolveImplementationsOf<T>(this TinyIoC.TinyIoCContainer container, object caller)
		{
			return caller.GetType().Assembly.GetTypes()
				.Where (t => t.GetInterfaces ().Any (i => i.Name == typeof(T).Name))
				.Select (t => (T)container.Resolve (t));
		}
	}
}

