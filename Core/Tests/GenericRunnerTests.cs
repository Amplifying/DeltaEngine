using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	/// <summary>
	/// Tests how to collect and inject generic runners with automatically injected dependencies.
	/// </summary>
	public class GenericRunnerTests
	{
		[Test]
		public void AddGenericClass()
		{
			var exampleRunner = new Example();
			AddRunner(exampleRunner);
			foreach (var runner in runners)
				foreach (var interfaceType in runner.GetType().GetInterfaces())
					InvokeGenericRun(interfaceType, runner);
			Assert.IsTrue(exampleRunner.WasInvoked);
			Assert.NotNull(new Injection());
		}

		private void AddRunner(object classToAdd)
		{
			if (classToAdd.GetType().GetInterfaces().Any(i => i.Name.StartsWith("Runner`")))
				runners.Add(classToAdd);
		}

		readonly List<object> runners = new List<object>();

		private static void InvokeGenericRun(Type interfaceType, object runner)
		{
			var arguments = interfaceType.GetGenericArguments();
			Type firstType = arguments[0];
			var newInstance = Activator.CreateInstance(firstType);
			MethodInfo genericMethod = runner.GetType().GetMethod("Run");
			genericMethod.Invoke(runner, new[] { newInstance });
		}

		internal class Injection { }

		internal class Example : Runner<Injection>
		{
			public void Run(Injection some)
			{
				WasInvoked = true;
			}

			public bool WasInvoked { get; private set; }
		}
	}
}