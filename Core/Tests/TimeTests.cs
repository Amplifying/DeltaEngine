using System;
using System.Threading;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	public class TimeTests
	{
		[Test]
		public void RunTime()
		{
			var time = new Time(new MockElapsedTime());
			time.Run();
			Assert.AreEqual(0, time.Milliseconds);
		}

		[Test]
		public void RunTimeWithStopwatch()
		{
			var time = new Time(new StopwatchTime());
			time.Run();
			Assert.AreEqual(0, time.Milliseconds);
		}

		[Test]
		public void CheckEveryWithInvalidStepAlwaysReturnsTrue()
		{
			var time = new Time(new MockElapsedTime());
			Assert.IsTrue(time.CheckEvery(-1));
		}

		[Test]
		public void CalculateFps()
		{
			var time = new Time(new MockElapsedTime());
			do
				time.Run();
			while (!time.CheckEvery(1.0f));
			Assert.IsTrue(Math.Abs(time.Fps - 10) <= 1, "Fps=" + time.Fps);
		}

		[Test]
		public void GetSecondsSinceStartToday()
		{
			var time = new Time(new MockElapsedTime());
			Assert.AreEqual(0.0f, time.GetSecondsSinceStartToday());
			time.Run();
			Assert.AreNotEqual(0.0f, time.GetSecondsSinceStartToday());
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void CalculateFpsWithStopwatch()
		{
			var time = new Time(new StopwatchTime());
			const int TargetFps = 50;
			do
			{
				Thread.Sleep(1000 / TargetFps);
				time.Run();
			} while (!time.CheckEvery(1.0f));
			Assert.IsTrue(Math.Abs(time.Fps - TargetFps) <= 4, "Fps=" + time.Fps);
		}
	}
}