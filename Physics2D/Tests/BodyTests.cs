using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	internal class BodyTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateAndSetValues()
		{
			var physics = Resolve<Physics>();
			physics.Gravity = Point.UnitY;
			Assert.AreEqual(Point.UnitY, physics.Gravity);
			var physicsBody = physics.CreateRectangle(new Size(0.5f));
			SetAndCheckLinearVelocity(physicsBody);
			SetAndCheckRotation(physicsBody);
			SetAndCheckPosition(physicsBody);
			SetAndCheckStatic(physicsBody);
			SetAndCheckRestitution(physicsBody);
			SetAndCheckFriction(physicsBody);
		}

		private static void SetAndCheckStatic(PhysicsBody physicsBody)
		{
			physicsBody.IsStatic = true;
			Assert.IsTrue(physicsBody.IsStatic);
		}

		private static void SetAndCheckRotation(PhysicsBody physicsBody)
		{
			physicsBody.Rotation = 30;
			Assert.AreEqual(30, physicsBody.Rotation);
		}

		private static void SetAndCheckPosition(PhysicsBody physicsBody)
		{
			physicsBody.Position = Point.Half;
			Assert.AreEqual(Point.Half, physicsBody.Position);
		}

		private static void SetAndCheckRestitution(PhysicsBody physicsBody)
		{
			physicsBody.Restitution = 1;
			Assert.AreEqual(1, physicsBody.Restitution);
		}

		private static void SetAndCheckFriction(PhysicsBody physicsBody)
		{
			physicsBody.Friction = 0.5f;
			Assert.AreEqual(0.5f, physicsBody.Friction);
		}

		private static void SetAndCheckLinearVelocity(PhysicsBody physicsBody)
		{
			physicsBody.LinearVelocity = new Point(5, -5);
			Assert.AreEqual(new Point(5, -5), physicsBody.LinearVelocity);
		}

		[Test]
		public void TestApplyLinearImpulse()
		{
			var body = Resolve<Physics>().CreateRectangle(new Size(45.0f, 45.0f));
			Point originalPosition = body.Position;
			body.ApplyLinearImpulse(Point.Zero);
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.AreNotEqual(originalPosition, body.Position);
		}

		[Test]
		public void TestApplyAngularImpulse()
		{
			var body = Resolve<Physics>().CreateRectangle(new Size(45.0f, 45.0f));
			float originalRotation = body.Rotation;
			body.ApplyAngularImpulse(10.0f);
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.AreNotEqual(originalRotation, body.Rotation);
		}

		[Test]
		public void TestApplyTorque()
		{
			var body = Resolve<Physics>().CreateRectangle(new Size(45.0f, 45.0f));
			float originalRotation = body.Rotation;
			body.ApplyTorque(10.0f);
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.AreNotEqual(originalRotation, body.Rotation);
		}

		[Test]
		public void CreateEdgeFromTwoSinglePoints()
		{
			Point[] edgePoints = { Point.Zero, Point.Half };
			var edge = Resolve<Physics>().CreateEdge(Point.Zero, Point.Half);
			Assert.AreEqual(edgePoints, edge.LineVertices);
		}

		[Test]
		public void CreateEdgeFromPointArray()
		{
			Point[] edgePoints = { Point.Zero, Point.Half };
			var edge = Resolve<Physics>().CreateEdge(edgePoints);
			Assert.AreEqual(edgePoints, edge.LineVertices);
		}

		[Test]
		public void CreateCircleAndGetVertices()
		{
			var body = Resolve<Physics>().CreateCircle(0.5f);
			Assert.IsNotEmpty(body.LineVertices);
		}

		[Test]
		public void CreatePolygonFromPoints()
		{
			Point[] polyPoints = { Point.Zero, Point.Half, Point.One, Point.UnitX };
			var body = Resolve<Physics>().CreatePolygon(polyPoints);
			Assert.IsNotEmpty(body.LineVertices);
		}

		[Test]
		public void DisposeBody()
		{
			var body = Resolve<Physics>().CreateRectangle(new Size(1));
			body.Dispose();
		}
	}
}