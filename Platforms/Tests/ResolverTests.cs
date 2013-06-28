using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Tests the Runner and Presenter classes manually without Platforms implementations.
	/// </summary>
	public sealed class ResolverTests : Resolver
	{
		[TestFixtureSetUp]
		public void CreateMockWindowAndDevice()
		{
			window = new MockWindow();
			RegisterInstance(window);
			device = new MockDevice();
			RegisterInstance(device);
			Assert.AreEqual(window, Resolve<MockWindow>());
			Assert.AreEqual(device, Resolve<MockDevice>());
		}

		private static readonly List<string> Output = new List<string>();
		private MockWindow window;
		private MockDevice device;

		private class MockWindow : Runner
		{
			public void Run()
			{
				Output.Add("Window.Run");
			}
		}

		private class MockDevice : Presenter
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

		[Test]
		public void ResolveUnknownClassAlwaysReturnsNull()
		{
			Assert.IsNull(Resolve<ResolverTests>());
			Assert.IsNull(Resolve(typeof(ResolverTests)));
		}

		[Test]
		public void RunnersAreExecutedInCorrectOrder()
		{
			Output.Clear();
			Run();
			const string ExpectedOutput = "Window.Run, Device.Run, AppRunnerTests, Device.Present";
			Assert.AreEqual(ExpectedOutput, Output.ToText());
		}

		private void RegisterInstance(object instance)
		{
			registeredInstances.Add(instance);
			RegisterInstanceAsRunnerOrPresenterIfPossible(instance);
		}

		private readonly List<object> registeredInstances = new List<object>();

		private void Run()
		{
			RunAllRunners();
			Output.Add("AppRunnerTests");
			RunAllPresenters();
		}

		internal override object Resolve(Type baseType, object customParameter = null)
		{
			if (customParameter != null)
				throw new CustomParameterNotSupportedYet();

			return null;
		}

		internal override BaseType Resolve<BaseType>()
		{
			foreach (BaseType instance in registeredInstances.OfType<BaseType>())
				return instance;

			return default(BaseType);
		}

		private class CustomParameterNotSupportedYet : Exception {}

		[Test]
		public void ResolveCustomNotSupported()
		{
			Assert.Throws<CustomParameterNotSupportedYet>(
				() => Resolve(typeof(Time), new StopwatchTime()));
		}

		[Test]
		public void EmptyConfigurationShouldNotCrash()
		{
			var resolver = new MockResolver();
			resolver.Resolve<MockDrawing>();
			resolver.Dispose();
		}

		[Test]
		public void RegisterDisposableInstance()
		{
			var instance = new SomeDisposableRunner();
			instance.Run();
			using (var newResolver = new ResolverTests())
				newResolver.RegisterInstance(instance);
			Assert.IsTrue(instance.DisposeWasCalled);
		}

		public class SomeDisposableRunner : Runner, IDisposable
		{
			public void Run() {}

			public void Dispose()
			{
				DisposeWasCalled = true;
			}

			public bool DisposeWasCalled { get; private set; }
		}

		[Test]
		public void RegisterDisposablePriorityRunner()
		{
			var instance = new SomeDisposablePriorityRunner();
			instance.Run();
			using (var newResolver = new ResolverTests())
				newResolver.RegisterInstance(instance);
			Assert.IsTrue(instance.DisposeWasCalled);
		}

		public class SomeDisposablePriorityRunner : PriorityRunner, IDisposable
		{
			public void Run() { }

			public void Dispose()
			{
				DisposeWasCalled = true;
			}

			public bool DisposeWasCalled { get; private set; }
		}

		[Test]
		public void RegisterGenericRunner()
		{
			var instance = new GenericRunner();
			instance.Run(Time.Current);
			RegisterInstance(instance);
		}

		private class GenericRunner : Runner<Time>
		{
			public void Run(Time first) {}
		}

		[Test]
		public void DisposeTwice()
		{
			Dispose();
			Dispose();
		}
	}
}