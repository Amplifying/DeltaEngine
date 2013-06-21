using DeltaEngine.Datatypes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace DeltaEngine.Physics2D.Farseer
{
	/// <summary>
	/// The Farseer physics implementation
	/// </summary>
	public class FarseerPhysics : Physics
	{
		public override PhysicsBody CreateCircle(float radius)
		{
			Body circle = BodyFactory.CreateCircle(world, unitConverter.ToSimUnits(radius), 1.0f);
			var body = new FarseerBody(circle) { UnitConverter = unitConverter };
			AddBody(body);
			return body;
		}

		private readonly World world = new World(new Vector2(0f, 9.82f));
		private readonly UnitConverter unitConverter = new UnitConverter(64f);

		public override PhysicsBody CreateRectangle(Size size)
		{
			Body rectangle = BodyFactory.CreateRectangle(world, unitConverter.ToSimUnits(size.Width),
				unitConverter.ToSimUnits(size.Height), 1.0f);
			var body = new FarseerBody(rectangle) { UnitConverter = unitConverter };
			AddBody(body);
			return body;
		}

		public override PhysicsBody CreateEdge(Point start, Point end)
		{
			var edge = BodyFactory.CreateEdge(world,
				unitConverter.ToSimUnits(unitConverter.Convert(start)),
				unitConverter.ToSimUnits(unitConverter.Convert(end)));
			var body = new FarseerBody(edge) { UnitConverter = unitConverter };
			AddBody(body);
			body.IsStatic = true;
			return body;
		}

		public override PhysicsBody CreateEdge(params Point[] vertices)
		{
			var edge = new Body(world);
			var fVertices = unitConverter.Convert(vertices);
			for (int i = 0; i < fVertices.Count - 1; ++i)
				FixtureFactory.AttachEdge(fVertices[i], fVertices[i + 1], edge);
			var body = new FarseerBody(edge) { UnitConverter = unitConverter };
			AddBody(body);
			return body;
		}

		public override PhysicsBody CreatePolygon(params Point[] vertices)
		{
			var polygon = unitConverter.Convert(vertices);
			var centroid = -polygon.GetCentroid();
			polygon.Translate(ref centroid);
			Body body = BodyFactory.CreatePolygon(world, polygon, 1.0f);
			var newBody = new FarseerBody(body) { UnitConverter = unitConverter };
			AddBody(newBody);
			return newBody;
		}

		public override PhysicsJoint CreateFixedAngleJoint(PhysicsBody body, float targetAngle)
		{
			var farseerJoint = JointFactory.CreateFixedAngleJoint(world, ((FarseerBody)body).Body);
			farseerJoint.TargetAngle = targetAngle;
			return new FarseerJoint(farseerJoint, body, body);
		}

		public override PhysicsJoint CreateAngleJoint(PhysicsBody bodyA, PhysicsBody bodyB,
			float targetAngle)
		{
			var farseerJoint = JointFactory.CreateAngleJoint(world, ((FarseerBody)bodyA).Body,
				((FarseerBody)bodyB).Body);
			farseerJoint.TargetAngle = targetAngle;
			return new FarseerJoint(farseerJoint, bodyA, bodyB);
		}

		public override PhysicsJoint CreateRevoluteJoint(PhysicsBody bodyA, PhysicsBody bodyB,
			Point anchor)
		{
			var farseerJoint = JointFactory.CreateRevoluteJoint(world, ((FarseerBody)bodyA).Body,
				((FarseerBody)bodyB).Body, unitConverter.Convert(anchor));
			return new FarseerJoint(farseerJoint, bodyA, bodyB);
		}

		public override PhysicsJoint CreateLineJoint(PhysicsBody bodyA, PhysicsBody bodyB, Point axis)
		{
			var farseerJoint = JointFactory.CreatePrismaticJoint(((FarseerBody)bodyA).Body,
				((FarseerBody)bodyB).Body, ((FarseerBody)bodyB).Body.Position,
				unitConverter.Convert(axis));
			world.AddJoint(farseerJoint);
			return new FarseerJoint(farseerJoint, bodyA, bodyB);
		}

		protected override void Simulate(float delta)
		{
			world.Step(delta);
		}

		public override Point Gravity
		{
			get { return unitConverter.Convert(world.Gravity); }
			set { world.Gravity = unitConverter.Convert(value); }
		}
	}
}