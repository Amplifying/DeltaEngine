using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Farseer.Tests
{
	public class PhysicsTests
	{
		[Test]
		public void IsNotPausedOnCreation()
		{
			var physics = new FarseerPhysics();
			Assert.IsFalse(physics.IsPaused);
		}

		[Test]
		public void DefaultGravity()
		{
			var physics = new FarseerPhysics();
			Assert.AreEqual(physics.Gravity, new Point(0f, 9.82f));
		}

		[Test]
		public void Gravity()
		{
			var physics = new FarseerPhysics();
			var gravity = Point.UnitY;
			physics.Gravity = gravity;
			Assert.AreEqual(physics.Gravity, gravity);
		}

		[Test]
		public void NoBodiesOnCreation()
		{
			var physics = new FarseerPhysics();
			Assert.AreEqual(0, physics.Bodies.Count());
		}

		[Test]
		public void OneBody()
		{
			var physics = new FarseerPhysics();
			var body = physics.CreateRectangle(Size.One);
			Assert.IsNotNull(body);
			Assert.AreEqual(1, physics.Bodies.Count());
		}

		[Test]
		public void CreateEdge()
		{
			var physics = new FarseerPhysics();
			var body = physics.CreateEdge(Point.Zero, Point.One);
			Assert.IsNotNull(body);
		}

		[Test]
		public void CreateEdgeMultiPoints()
		{
			var physics = new FarseerPhysics();
			var body =
				physics.CreateEdge(new[] { Point.Zero, Point.One, Point.Half, Point.UnitX, Point.UnitY });
			Assert.IsNotNull(body);
		}

		[Test]
		public void CreatePolygon()
		{
			var physics = new FarseerPhysics();
			var body =
				physics.CreatePolygon(new[] { Point.Zero, Point.One, Point.Half, Point.UnitX, Point.UnitY });
			Assert.IsNotNull(body);
		}
	}
}