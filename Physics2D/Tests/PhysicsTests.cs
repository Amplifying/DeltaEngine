using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	public class PhysicsTests : TestWithAllFrameworks
	{
		[Test]
		public void IsNotPausedOnStartup()
		{
			var physics = new MockPhysics();
			Assert.IsFalse(physics.IsPaused);
		}

		[Test]
		public void DefaultGravity()
		{
			var physics = new MockPhysics();
			Assert.AreEqual(new Point(0f, 9.82f), physics.Gravity);
		}

		[Test]
		public void Gravity()
		{
			var physics = new MockPhysics { Gravity = Point.UnitY };
			Assert.AreEqual(Point.UnitY, physics.Gravity);
		}

		[Test]
		public void NoBodiesInitially()
		{
			var physics = new MockPhysics();
			var bodiesCount = physics.Bodies.Count();
			Assert.AreEqual(0, bodiesCount);
		}

		[Test]
		public void CreateOneBody()
		{
			var physics = new MockPhysics();
			Assert.IsNotNull(physics.CreateRectangle(Size.One));
			Assert.AreEqual(1, physics.Bodies.Count());
		}

		[Test]
		public void CreateEdge()
		{
			var physics = new MockPhysics();
			Assert.IsNotNull(physics.CreateEdge(Point.Zero, Point.One));
			Assert.IsNotNull(
				physics.CreateEdge(new[] { Point.Zero, Point.One, Point.Half, Point.UnitX, Point.UnitY }));
		}

		[Test]
		public void CreatePolygon()
		{
			var physics = new MockPhysics();
			Assert.IsNotNull(
				physics.CreatePolygon(new[] { Point.Zero, Point.One, Point.Half, Point.UnitX, Point.UnitY }));
		}

		[Test]
		public void SimulateMocKPhysics()
		{
			Start(typeof(MockResolver), (MockPhysics physics) =>
			{
				var body = physics.CreateRectangle(new Size(1));
				body.Position = Point.Zero;
				body.LinearVelocity = Point.One;
				mockResolver.AdvanceTimeAndExecuteRunners();
				Assert.AreEqual(Point.Zero, body.Position);
			});
		}
	}
}