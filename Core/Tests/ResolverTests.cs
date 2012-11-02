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
			Assert.AreEqual(null, Resolve<RunnerTests>());
			RunAllRunners();
			RunAllPresenters();
		}

		private void RegisterInstance(object instance)
		{
			registeredInstances.Add(instance);
			RegisterInstanceAsRunnerOrPresenterIfPossible(instance);
		}

		private readonly List<object> registeredInstances = new List<object>();

		private BaseType Resolve<BaseType>()
		{
			foreach (BaseType instance in registeredInstances.OfType<BaseType>())
				return instance;

			return default(BaseType);
		}
	}
}