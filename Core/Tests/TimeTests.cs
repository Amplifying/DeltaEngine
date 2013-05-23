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
			var time = new MockTime();
			time.Run();
			Assert.AreEqual(0, time.Milliseconds);
		}

		[Test]
		public void RunTimeWithStopwatch()
		{
			var time = new StopwatchTime();
			time.Run();
			Assert.IsTrue(time.Milliseconds < 2);
		}
		
		[Test]
		public void GetCurrentDelta()
		{
			Time.Current.Run();
			Assert.IsTrue(Time.Current.Delta < 2);
		}

		[Test]
		public void CheckEveryWithInvalidStepAlwaysReturnsTrue()
		{
			var time = new MockTime();
			Assert.IsTrue(time.CheckEvery(-1));
		}

		[Test]
		public void CalculateFps()
		{
			var time = new MockTime();
			do
				time.Run();
			while (!time.CheckEvery(1.0f));
			Assert.IsTrue(Math.Abs(time.Fps - 10) <= 1, "Fps=" + time.Fps);
		}

		[Test]
		public void GetSecondsSinceStartToday()
		{
			var time = new MockTime();
			Assert.AreEqual(0.0f, time.GetSecondsSinceStartToday());
			time.Run();
			Assert.AreNotEqual(0.0f, time.GetSecondsSinceStartToday());
		}
		
		[Test]
		public void FpsShouldBeSixty()
		{
			Assert.AreEqual(60, Time.Current.Fps);
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void CalculateFpsWithStopwatch()
		{
			var time = new StopwatchTime();
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