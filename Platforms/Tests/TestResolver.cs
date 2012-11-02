using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using Moq;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class TestResolver : AutofacResolver
	{
		public TestResolver()
		{
			var windowMock = RegisterMock<Window>();
			SetupWindowAndMakeSureAppIsClosedAfterOneFrame(windowMock);
			var deviceMock = RegisterMock<Device>();
			var drawingInstance = new Mock<Drawing>(deviceMock.Object).Object;
			var draw = RegisterMock(drawingInstance);
			Assert.AreEqual(draw, RegisterMock(drawingInstance));
			RegisterMock(new Mock<Renderer>(draw).Object);
			RegisterMock(new Mock<Time>(new Mock<ElapsedTime>().Object).Object);
		}

		private Mock<T> RegisterMock<T>()
			where T : class
		{
			var mock = new Mock<T>();
			RegisterMock(mock.Object);
			return mock;
		}

		private T RegisterMock<T>(T instance)
			where T : class
		{
			Type instanceType = instance.GetType();
			if (registeredMocks.Any(mock => mock.GetType() == instanceType))
				return instance;

			registeredMocks.Add(instance);
			alreadyRegisteredTypes.AddRange(instanceType.GetInterfaces());
			RegisterInstance(instance);
			return instance;
		}
		
		private readonly List<object> registeredMocks = new List<object>();

		private static void SetupWindowAndMakeSureAppIsClosedAfterOneFrame(Mock<Window> windowMock)
		{
			windowMock.Setup(window => window.IsVisible).Returns(true);
			windowMock.Setup(window => window.IsClosing).Returns(true);
			windowMock.Setup(window => window.Title).Returns("WindowMock");
			windowMock.Setup(window => window.TotalSize).Returns(new Size(800, 600));
			windowMock.Setup(window => window.ViewportSize).Returns(new Size(800, 600));
		}

		protected override void MakeSureContainerIsInitialized()
		{
			if (IsAlreadyInitialized)
				return;

			foreach (var instance in registeredMocks)
				AddRunnerAndPresenterForExistingInstance(instance);

			base.MakeSureContainerIsInitialized();
			testElapsedMs = GetTimeInMsForSlowTests();
		}

		private void AddRunnerAndPresenterForExistingInstance(object instance)
		{
			if (instance is Runner)
				runners.Add(instance as Runner);
			if (instance is Presenter)
				presenters.Add(instance as Presenter);
		}

		private long testElapsedMs;

		private long GetTimeInMsForSlowTests()
		{
			if (testStarted != null)
				return testStarted.ElapsedMilliseconds;

			testStarted = new Stopwatch();
			testStarted.Start();
			return 0;
		}

		private Stopwatch testStarted;

		public override void Dispose()
		{
			WarnIfUnitTestTakesTooLong();
			base.Dispose();
		}

		//ncrunch: no coverage start
		private void WarnIfUnitTestTakesTooLong()
		{
			if (StackTraceExtensions.ContainsUnitTest() && TookLongerThan10Ms())
				Debug.WriteLine("This unit test takes too long (" + testElapsedMs + "ms, max. 10ms is " +
					"allowed), please add Category(\"Slow\") to run it nightly instead!");
		}

		private bool TookLongerThan10Ms()
		{
			testElapsedMs = GetTimeInMsForSlowTests() - testElapsedMs;
			return testElapsedMs > 10;
		}
	}
}