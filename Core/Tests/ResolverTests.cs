using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	/// <summary>
	/// Tests the Runner and Presenter classes manually without DeltaEngine.Configuration.
	/// </summary>
	public sealed class ResolverTests : Resolver
	{
		[Test]
		public void ResolveRunner()
		{
			var window = new RunnerTests.Window();
			RegisterInstance(window);
			var device = new RunnerTests.Device(window);
			RegisterInstance(device);

			Assert.AreEqual(window, Resolve<RunnerTests.Window>());
			Assert.AreEqual(device, Resolve<RunnerTests.Device>());
			Assert.IsNull(Resolve<RunnerTests>());
			Assert.IsNull(Resolve(typeof(RunnerTests)));
			RunAllRunners();
			RunAllPresenters();
		}

		private void RegisterInstance(object instance)
		{
			registeredInstances.Add(instance);
			RegisterInstanceAsRunnerOrPresenterIfPossible(instance);
		}

		private readonly List<object> registeredInstances = new List<object>();

		protected override object Resolve(Type baseType)
		{
			return null;
		}

		public override BaseType Resolve<BaseType>(object customParameter = null)
		{
			if (customParameter != null)
				throw new CustomParameterNotSupportedYet();

			foreach (BaseType instance in registeredInstances.OfType<BaseType>())
				return instance;

			return default(BaseType);
		}

		private class CustomParameterNotSupportedYet : Exception {}

		[Test]
		public void ResolveCustomNotSupported()
		{
			Assert.Throws<CustomParameterNotSupportedYet>(
				() => Resolve<Time>(new object[] { new StopwatchTime() }));
		}

		[Test]
		public void DisposableInstance()
		{
			RegisterInstance(new SomeDisposableRunner());
			Dispose();
		}

		public class SomeDisposableRunner : Runner, IDisposable
		{
			public void Dispose() { }
			public void Run() {}
		}
	}
}