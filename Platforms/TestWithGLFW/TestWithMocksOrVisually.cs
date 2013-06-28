using System;
using DeltaEngine.Core;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Automatically tests with MockResolver when NCrunch is used, otherwise GLFWResolver is used.
	/// </summary>
	[TestFixture]
	public class TestWithMocksOrVisually
	{
		[SetUp]
		public void InitializeResolver()
		{
			RunCode = null;
			remWindow = null;
			if (StackTraceExtensions.StartedFromNCrunch)
				resolver = new MockResolver();
				//ncrunch: no coverage start
			else
			{
				try
				{
					StackTraceExtensions.SetUnitTestName(TestContext.CurrentContext.Test.FullName);
				}
				catch {}
				resolver = new GLFWResolver();
			}
			//ncrunch: no coverage end
			resolver.CreateEntitySystemAndAddAsRunner();
		}

		protected AutofacStarter resolver;

		[TearDown]
		public void RunTestAndDisposeResolverWhenDone()
		{
			if (StackTraceExtensions.IsStartedFromNunitConsole())
				Window.CloseAfterFrame(); //ncrunch: no coverage

			try
			{
				if (TestContext.CurrentContext.Result.Status == TestStatus.Passed)
					resolver.Run(RunCode);
			}
			catch
			{
				resolver.Run(RunCode);
			}

			resolver.Dispose();
		}

		protected Window Window
		{
			get
			{
				if (remWindow != null)
					return remWindow;
				resolver.Resolve<Device>();
				return resolver.Resolve<Window>();
			}
		}

		private Window remWindow;

		protected Settings Settings
		{
			get { return resolver.Resolve<Settings>(); }
		}

		protected InputCommands Input
		{
			get
			{
				return resolver.Resolve<InputCommands>();
			}
		}
		protected Action RunCode;

		protected T Resolve<T>()
		{
			return resolver.Resolve<T>();
		}
	}
}