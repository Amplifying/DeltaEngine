using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	internal class FarseerJointTests : TestWithAllFrameworks
	{
		private void InitPileOfBodies(Physics physics)
		{
			testBodyAlpha = physics.CreateRectangle(new Size(1));
			testBodyBeta = physics.CreateCircle(1);
		}

		private PhysicsBody testBodyAlpha, testBodyBeta;

		[Test]
		public void CreateLineJointAndSetValues()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				InitPileOfBodies(physics);
				var joint = physics.CreateLineJoint(testBodyAlpha, testBodyBeta, Point.One);
				Assert.NotNull(joint);
				joint.Frequency = 0.5f;
				joint.MaxMotorTorque = 3;
				joint.MotorEnabled = true;
				joint.MotorSpeed = 5;
				Assert.AreEqual(0.5f, joint.Frequency);
				Assert.AreEqual(3, joint.MaxMotorTorque);
				Assert.IsTrue(joint.MotorEnabled);
				Assert.AreEqual(5, joint.MotorSpeed);
			});
		}

		[Test]
		public void CreateFixAngleJoint()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				InitPileOfBodies(physics);
				var joint = physics.CreateFixedAngleJoint(testBodyAlpha, 50);
				Assert.NotNull(joint);
			});
		}

		[Test]
		public void CreateAngleJoint()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				InitPileOfBodies(physics);
				var joint = physics.CreateAngleJoint(testBodyAlpha, testBodyBeta, 50);
				Assert.NotNull(joint);
			});
		}

		[Test]
		public void CreateRevoluteJoint()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				InitPileOfBodies(physics);
				var joint = physics.CreateRevoluteJoint(testBodyAlpha, testBodyBeta, Point.One);
				Assert.NotNull(joint);
			});
		}
	}
}