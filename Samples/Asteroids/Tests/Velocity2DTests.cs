using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class Velocity2DTests : TestWithAllFrameworks
	{
		[SetUp]
		public void InitializeVelocity()
		{
			velocity = new Velocity2D(Point.Zero, 5);
		}

		private Velocity2D velocity;

		[Test]
		public void AccelerateByPoint()
		{
			velocity.Accelerate(Point.One);
			Assert.AreEqual(Point.One, velocity.velocity);
		}

		[Test]
		public void AccelerateByPointExceedingMaximum()
		{
			velocity.Accelerate(new Point(6, 0));
			Assert.AreEqual(5, velocity.velocity.X);
			Assert.AreEqual(0, velocity.velocity.Y);
		}

		[Test]
		public void AccelerateByMagnitudeAngle()
		{
			velocity.Accelerate(4, 0);
			Assert.AreEqual(-4, velocity.velocity.Y);
			Assert.AreEqual(0, velocity.velocity.X);
		}

		[Test]
		public void AccelerateByMagnitudeAngleExceedingMaximum()
		{
			velocity.Accelerate(6, 0);
			Assert.AreEqual(-5, velocity.velocity.Y);
			Assert.AreEqual(0, velocity.velocity.X);
		}
	}
}