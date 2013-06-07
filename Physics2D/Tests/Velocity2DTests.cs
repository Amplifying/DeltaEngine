using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
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

		[Test]
		public void AccelerateByScalarFactor()
		{
			velocity.Accelerate(Point.One);
			velocity.Accelerate(2);
			Assert.AreEqual(2,velocity.velocity.X);
			Assert.AreEqual(2,velocity.velocity.Y);
		}

		[Test]
		public void AccelerateByScalarFactorExceedingMaximum()
		{
			velocity.Accelerate(Point.UnitX);
			velocity.Accelerate(-7.0f);
			Assert.AreEqual(-5,velocity.velocity.X);
			Assert.AreEqual(0,velocity.velocity.Y);
		}
	}
}