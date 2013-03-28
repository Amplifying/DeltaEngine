using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class TimeTests
	{
		[SetUp]
		public void InitializeTimeTests()
		{
			resolver = new TestResolver();
			time = resolver.Resolve<Time>();
		}

		private TestResolver resolver;
		private Time time;

		[Test]
		public void InitiallyElapsedTimeShouldBeZero()
		{
			Assert.AreEqual(0, time.Milliseconds);
			Assert.IsTrue(time.GetSecondsSinceStartToday() > 0.0f);
		}

		[Test]
		public void TimeDeltaShouldBeOneSixtiesOfASecond()
		{
			Assert.IsTrue(time.CurrentDelta.IsNearlyEqual(1.0f / ExpectedFps, 0.01f),
				"time.CurrentDelta should be " + (1.0f / ExpectedFps) + ", but is " + time.CurrentDelta);
		}

		private const int ExpectedFps = 60;

		[Test]
		public void FpsShouldBeSixty()
		{
			Assert.AreEqual(60, time.Fps);
		}

		[Test]
		public void AdvanceTimeOneTick()
		{
			time.Run();
			Assert.AreEqual(1000 / ExpectedFps, time.Milliseconds);
			Assert.IsTrue(time.GetSecondsSinceStartToday() > 0.016f);
			TimeDeltaShouldBeOneSixtiesOfASecond();
		}

		[Test]
		public void AdvanceTimeOneTensOfASecond()
		{
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			time.Run();
			Assert.IsTrue(time.Milliseconds >= 100 && time.Milliseconds <= 120);
			TimeDeltaShouldBeOneSixtiesOfASecond();
		}

		[Test]
		public void AdvanceTimeOneSecond()
		{
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.IsTrue(time.Milliseconds >= 1000);
			Assert.IsTrue(time.GetSecondsSinceStartToday() > 1.0f);
			TimeDeltaShouldBeOneSixtiesOfASecond();
			FpsShouldBeSixty();
		}
	}
}