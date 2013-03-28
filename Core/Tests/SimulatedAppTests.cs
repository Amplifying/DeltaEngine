using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	/// <summary>
	/// Mocks resolver, window and device to test execution order of Runners and Presenters.
	/// </summary>
	public class SimulatedAppTests
	{
		private static readonly List<string> Output = new List<string>();

		private sealed class TestResolver : Resolver
		{
			public TestResolver()
			{
				RegisterInstance(new Window());
				RegisterInstance(new Device());
				Assert.IsNull(Resolve(null));
			}

			private void RegisterInstance(object instance)
			{
				RegisterInstanceAsRunnerOrPresenterIfPossible(instance);
			}

			public void Run()
			{
				RunAllRunners();
				Output.Add("AppRunnerTests");
				RunAllPresenters();
			}

			protected override object Resolve(Type baseType)
			{
				return null;
			}

			public override BaseType Resolve<BaseType>(object customParameter = null)
			{
				return default(BaseType);
			}
		}

		private class Device : Presenter
		{
			public void Run()
			{
				Output.Add("Device.Run");
			}

			public void Present()
			{
				Output.Add("Device.Present");
			}
		}

		private class Window : Runner
		{
			public void Run()
			{
				Output.Add("Window.Run");
			}
		}

		[Test]
		public void RunnersAreExecutedInCorrectOrder()
		{
			Output.Clear();
			using (var resolver = new TestResolver())
				resolver.Run();

			const string ExpectedOutput = "Window.Run, Device.Run, AppRunnerTests, Device.Present";
			Assert.AreEqual(ExpectedOutput, Output.ToText());
		}

		[Test]
		public void EmptyConfigurationShouldNotCrash()
		{
			var resolver = new TestResolver();
			resolver.Resolve<Device>();
			resolver.Dispose();
		}
	}
}