namespace DeltaEngine.Physics2D.Tests
{
	internal class MockPhysicsJoint : PhysicsJoint
	{
		public MockPhysicsJoint(PhysicsBody bodyA, PhysicsBody bodyB)
			: base(bodyA, bodyB) {}

		public override float MotorSpeed { get; set; }
		public override float MaxMotorTorque { get; set; }
		public override bool MotorEnabled { get; set; }
		public override float Frequency { get; set; }
	}
}