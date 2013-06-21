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
		public void IsNotPausedOnCreation()
		{
			Start(typeof(MockResolver), (Physics physics) => { Assert.IsFalse(physics.IsPaused); });
		}

		[Test]
		public void DefaultGravity()
		{
			Start(typeof(MockResolver),
				(Physics physics) => { Assert.AreEqual(physics.Gravity, new Point(0f, 9.82f)); });
		}

		[Test]
		public void Gravity()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var gravity = Point.UnitY;
				physics.Gravity = gravity;
				Assert.AreEqual(physics.Gravity, gravity);
			});
		}

		[Test]
		public void NoBodiesOnCreation()
		{
			Start(typeof(MockResolver),
				(Physics physics) => { Assert.AreEqual(0, physics.Bodies.Count()); });
		}

		[Test]
		public void OneBody()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var body = physics.CreateRectangle(Size.One);
				Assert.IsNotNull(body);
				Assert.AreEqual(1, physics.Bodies.Count());
			});
		}

		[Test]
		public void CreateEdge()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var body = physics.CreateEdge(Point.Zero, Point.One);
				Assert.IsNotNull(body);
			});
		}

		[Test]
		public void CreateEdgeMultiPoints()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var body =
					physics.CreateEdge(new[] { Point.Zero, Point.One, Point.Half, Point.UnitX, Point.UnitY });
				Assert.IsNotNull(body);
			});
		}

		[Test]
		public void CreatePolygon()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var body =
					physics.CreatePolygon(new[] { Point.Zero, Point.One, Point.Half, Point.UnitX, Point.UnitY });
				Assert.IsNotNull(body);
			});
		}
	}
}