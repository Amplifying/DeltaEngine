using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	public class PhysicsTests : TestWithMocksOrVisually
	{
		[Test]
		public void IsNotPausedOnCreation()
		{
			Assert.IsFalse(Resolve<Physics>().IsPaused);
		}

		[Test]
		public void DefaultGravity()
		{
			Assert.AreEqual(Resolve<Physics>().Gravity, new Point(0f, 9.82f));
		}

		[Test]
		public void Gravity()
		{
			var gravity = Point.UnitY;
			Resolve<Physics>().Gravity = gravity;
			Assert.AreEqual(Resolve<Physics>().Gravity, gravity);
		}

		[Test]
		public void NoBodiesOnCreation()
		{
			Assert.AreEqual(0, Resolve<Physics>().Bodies.Count());
		}

		[Test]
		public void OneBody()
		{
			var body = Resolve<Physics>().CreateRectangle(Size.One);
			Assert.IsNotNull(body);
			Assert.AreEqual(1, Resolve<Physics>().Bodies.Count());
		}

		[Test]
		public void CreateEdge()
		{
			var body = Resolve<Physics>().CreateEdge(Point.Zero, Point.One);
			Assert.IsNotNull(body);
		}

		[Test]
		public void CreateEdgeMultiPoints()
		{
			var body =
				Resolve<Physics>().CreateEdge(new[]
				{ Point.Zero, Point.One, Point.Half, Point.UnitX, Point.UnitY });
			Assert.IsNotNull(body);
		}

		[Test]
		public void CreatePolygon()
		{
			var body =
				Resolve<Physics>().CreatePolygon(new[]
				{ Point.Zero, Point.One, Point.Half, Point.UnitX, Point.UnitY });
			Assert.IsNotNull(body);
		}
	}
}