using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Physics2D
{
	/// <summary>
	/// Main physics class used for performing simulations and creating bodies and shapes.
	/// </summary>
	public abstract class Physics : PriorityRunner
	{
		public abstract PhysicsBody CreateCircle(float radius);
		public abstract PhysicsBody CreateRectangle(Size size);
		public abstract PhysicsBody CreateEdge(Point start, Point end);
		public abstract PhysicsBody CreateEdge(params Point[] vertices);
		public abstract PhysicsBody CreatePolygon(params Point[] vertices);
		public abstract PhysicsJoint CreateFixedAngleJoint(PhysicsBody body, float targetAngle);

		public abstract PhysicsJoint CreateAngleJoint(PhysicsBody bodyA, PhysicsBody bodyB,
			float targetAngle);

		public abstract PhysicsJoint CreateRevoluteJoint(PhysicsBody bodyA, PhysicsBody bodyB,
			Point anchor);

		public abstract PhysicsJoint CreateLineJoint(PhysicsBody bodyA, PhysicsBody bodyB, Point axis);
		public abstract Point Gravity { get; set; }
		public bool IsPaused { get; set; }

		public void Run()
		{
			if (!IsPaused)
				Simulate(Time.Current.Delta);
		}

		protected abstract void Simulate(float delta);

		protected void AddBody(PhysicsBody body)
		{
			bodies.Add(body);
		}

		private readonly List<PhysicsBody> bodies = new List<PhysicsBody>();
		public IEnumerable<PhysicsBody> Bodies
		{
			get { return bodies; }
		}
	}
}