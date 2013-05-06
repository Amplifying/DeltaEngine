using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Logging;
using DeltaEngine.Platforms.Tests.ModuleMocks;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class AppStarterTests : TestWithMockResolver
	{
		[SetUp]
		public void CreateTestResolver()
		{
			app = new AutofacStarterForMockResolver();
			var windowMock = app.RegisterMock<Window>();
			windowMock.Setup(w => w.Visibility).Returns(true);
			windowMock.Setup(w => w.IsClosing).Returns(true);
			new LoggingMocks(app);
		}

		private AutofacStarterForMockResolver app;

		[Test]
		public void TestExceptionLoggingInInitialization()
		{
			app.Start<MockEntryRunnerThatThrowsExceptionInConstructor, FirstClass>();
			var logger = app.Resolve<Logger>();
			var lastMessage = logger.LastMessage;
			Assert.NotNull(lastMessage);
			Assert.That(lastMessage, Is.TypeOf<Error>());
			Assert.IsTrue(lastMessage.Text.Contains("Test Exception"));
		}

		[Test]
		public void TestExceptionLoggingInRun()
		{
			app.Start<MockEntryRunnerThatThrowsExceptionInRun, FirstClass>();
			var logger = app.Resolve<Logger>();
			var lastMessage = logger.LastMessage;
			Assert.NotNull(lastMessage);
			Assert.That(lastMessage, Is.TypeOf<Error>());
			Assert.IsTrue(lastMessage.Text.Contains("Test Exception"));
		}

		[Test]
		public void TestEntityRegistration()
		{
			app.Register<Line2D>();
			app.Start<MockEntryRunner, FirstClass>();
		}

		[Test]
		public void StartAppWithOneClassToRegister()
		{
			app.Start<MockEntryRunner, FirstClass>();
			var firstClass = app.Resolve<FirstClass>();
			Assert.IsNotNull(firstClass);
		}

		[Test]
		public void StartAppWithTwoClassToRegister()
		{
			app.Start<MockEntryRunner, FirstClass, SecondClass>();
			var firstClass = app.Resolve<FirstClass>();
			Assert.IsNotNull(firstClass);
			var secondClass = app.Resolve<SecondClass>();
			Assert.IsNotNull(secondClass);
		}

		[Test]
		public void StartAppWithListOfTypesToRegister()
		{
			var typeList = new List<Type> { typeof(FirstClass), typeof(SecondClass) };
			app.Start<MockEntryRunner>(typeList);
			var firstClass = app.Resolve<FirstClass>();
			Assert.IsNotNull(firstClass);
			var secondClass = app.Resolve<SecondClass>();
			Assert.IsNotNull(secondClass);
		}

		public class MockEntryRunner : Runner
		{
			public void Run() {}
		}

		public class MockEntryRunnerThatThrowsExceptionInConstructor : Runner
		{
			public MockEntryRunnerThatThrowsExceptionInConstructor()
			{
				throw new Exception("Test Exception");
			}

			public void Run() {}
		}

		public class MockEntryRunnerThatThrowsExceptionInRun : Runner
		{
			public void Run()
			{
				throw new Exception("Test Exception");
			}
		}

		private class FirstClass {}

		private class SecondClass {}
	}
}