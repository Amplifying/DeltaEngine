using DeltaEngine.Datatypes;

namespace DeltaEngine.Physics2D.Tests
{
	public class MockPhysicsBody : PhysicsBody
	{
		private const float DefaultFriction = 0.2f;

		public MockPhysicsBody()
		{
			Friction = DefaultFriction;
		}

		public MockPhysicsBody(bool wantNullVertices)
			: this()
		{
			this.wantNullVertices = wantNullVertices;
		}
		private readonly bool wantNullVertices;

		public Point Position { get; set; }
		public bool IsStatic { get; set; }
		public float Restitution { get; set; }
		public float Friction { get; set; }
		public float Rotation { get; set; }
		public void ApplyLinearImpulse(Point impulse) { }
		public void ApplyAngularImpulse(float impulse) { }
		public void ApplyTorque(float torque) { }
		public Point[] LineVertices
		{
			get { return wantNullVertices ? null : new Point[0]; }
		}
	}
}