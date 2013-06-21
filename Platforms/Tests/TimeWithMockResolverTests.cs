using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class TimeWithMockResolverTests
	{
		[SetUp]
		public void InitializeTimeTests()
		{
			resolver = new MockResolver();
		}

		private MockResolver resolver;

		[Test]
		public void TimeDeltaShouldBeOneSixtiesOfASecond()
		{
			Assert.IsTrue(Time.Current.Delta.IsNearlyEqual(1.0f / ExpectedFps, 0.01f),
				"Time.Current.Delta should be " + (1.0f / ExpectedFps) + ", but is " + Time.Current.Delta);
		}

		private const int ExpectedFps = 60;

		[Test]
		public void AdvanceTimeOneTick()
		{
			Time.Current.Run();
			Assert.LessOrEqual(16, Time.Current.Milliseconds);
			Assert.Less(Time.Current.GetSecondsSinceStartToday(), 0.04f);
		}

		[Test]
		public void AdvanceTimeOneTensOfASecond()
		{
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.GreaterOrEqual(Time.Current.Milliseconds, 100);
		}

		[Test]
		public void AdvanceTimeOneSecond()
		{
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Time.Current.Run();
			Assert.GreaterOrEqual(Time.Current.Milliseconds, 1000);
			Assert.Greater(Time.Current.GetSecondsSinceStartToday(), 1.0f);
		}
	}
}