using DeltaEngine.Datatypes;

namespace DeltaEngine.Physics2D.Tests
{
	public class MockPhysics : Physics
	{
		public MockPhysics()
		{
			gravity = new Point(0f, 9.82f);
		}
		private Point gravity;

		public override PhysicsBody CreateCircle(float radius)
		{
			return CreateAndAddBody();
		}

		private PhysicsBody CreateAndAddBody()
		{
			var newBody = new MockPhysicsBody();
			AddBody(newBody);
			return newBody;
		}

		public override PhysicsBody CreateRectangle(Size size)
		{
			return CreateAndAddBody();
		}

		public override PhysicsBody CreateEdge(Point start, Point end)
		{
			return CreateAndAddBody();
		}

		public override PhysicsBody CreateEdge(params Point[] vertices)
		{
			return CreateAndAddBody();
		}

		public override PhysicsBody CreatePolygon(params Point[] vertices)
		{
			return CreateAndAddBody();
		}

		public override PhysicsJoint CreateFixedAngleJoint(PhysicsBody body, float targetAngle)
		{
			return new MockPhysicsJoint(body, body);
		}

		public override PhysicsJoint CreateAngleJoint(PhysicsBody bodyA, PhysicsBody bodyB,
			float targetAngle)
		{
			return new MockPhysicsJoint(bodyA, bodyB);
		}

		public override PhysicsJoint CreateRevoluteJoint(PhysicsBody bodyA, PhysicsBody bodyB,
			Point anchor)
		{
			return new MockPhysicsJoint(bodyA, bodyB);
		}

		public override PhysicsJoint CreateLineJoint(PhysicsBody bodyA, PhysicsBody bodyB, Point axis)
		{
			return new MockPhysicsJoint(bodyA, bodyB);
		}

		public override Point Gravity
		{
			get { return gravity; }
			set { gravity = value; }
		}

		protected override void Simulate(float delta) {}
	}
}