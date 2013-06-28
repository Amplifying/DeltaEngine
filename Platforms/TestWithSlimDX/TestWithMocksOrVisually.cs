using System;
using DeltaEngine.Core;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Automatically tests with MockResolver when NCrunch is used, otherwise SlimDXResolver is used.
	/// </summary>
	[TestFixture]
	public class TestWithMocksOrVisually
	{
		[SetUp]
		public void InitializeResolver()
		{
			if (StackTraceExtensions.StartedFromNCrunch)
				resolver = new MockResolver();
			//ncrunch: no coverage start
			else
			{
				StackTraceExtensions.SetUnitTestName(TestContext.CurrentContext.Test.FullName);
				resolver = new SlimDXResolver();
			}
			//ncrunch: no coverage end
			resolver.CreateEntitySystemAndAddAsRunner();
		}

		private AutofacStarter resolver;

		[TearDown]
		public void RunTestAndDisposeResolverWhenDone()
		{
			if (StackTraceExtensions.IsStartedFromNunitConsole())
				Window.CloseAfterFrame(); //ncrunch: no coverage
			if (TestContext.CurrentContext.Result.Status == TestStatus.Passed)
				resolver.Run(RunCode);
			resolver.Dispose();
		}

		protected Window Window
		{
			get
			{
				resolver.Resolve<Device>();
				return resolver.Resolve<Window>();
			}
		}

		protected Settings Settings
		{
			get { return resolver.Resolve<Settings>(); }
		}

		protected Action RunCode;

		protected T Resolve<T>()
		{
			return resolver.Resolve<T>();
		}
	}
}