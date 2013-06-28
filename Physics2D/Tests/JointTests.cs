using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	public class JointTests : TestWithMocksOrVisually
	{
		[Test]
		public void TestFixedAngleJoint()
		{
			var body = Resolve<Physics>().CreateCircle(3.0f);
			var joint = Resolve<Physics>().CreateFixedAngleJoint(body, (float)Math.PI / 3);
			Assert.IsNotNull(joint);
		}

		[Test]
		public void TestFixedAngleJointSameBodies()
		{
			var body = Resolve<Physics>().CreateCircle(3.0f);
			var joint = Resolve<Physics>().CreateFixedAngleJoint(body, (float)Math.PI / 3);
			Assert.AreEqual(joint.BodyA, body);
			Assert.AreEqual(joint.BodyB, body);
		}

		[Test]
		public void TestAngleJoint()
		{
			var physics = Resolve<Physics>();
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateAngleJoint(bodyA, bodyB, (float)Math.PI / 2);
			Assert.IsNotNull(joint);
		}

		[Test]
		public void TestAngleJointBodiesEqual()
		{
			var physics = Resolve<Physics>();
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateAngleJoint(bodyA, bodyB, (float)Math.PI / 2);
			Assert.AreEqual(joint.BodyA, bodyA);
			Assert.AreEqual(joint.BodyB, bodyB);
		}

		[Test]
		public void TestRevoluteJoint()
		{
			var physics = Resolve<Physics>();
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateRevoluteJoint(bodyA, bodyB, Point.Zero);
			Assert.IsNotNull(joint);
		}

		[Test]
		public void TestRevoluteJointBodiesEqual()
		{
			var physics = Resolve<Physics>();
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateRevoluteJoint(bodyA, bodyB, Point.Zero);
			Assert.AreEqual(joint.BodyA, bodyA);
			Assert.AreEqual(joint.BodyB, bodyB);
		}

		[Test]
		public void TestLineJointMotorEnabled()
		{
			var physics = Resolve<Physics>();
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateLineJoint(bodyA, bodyB, Point.Zero);
			Assert.AreEqual(joint.MotorEnabled, false);
			joint.MotorEnabled = true;
			Assert.AreEqual(joint.MotorEnabled, true);
		}

		[Test]
		public void TestLineJointMaxMotorTorque()
		{
			var physics = Resolve<Physics>();
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateLineJoint(bodyA, bodyB, Point.Zero);
			joint.MaxMotorTorque = 1.0f;
			Assert.AreEqual(joint.MaxMotorTorque, 1.0f);
		}

		[Test]
		public void TestLineJointMotorSpeed()
		{
			var physics = Resolve<Physics>();
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateLineJoint(bodyA, bodyB, Point.Zero);
			joint.MotorSpeed = 4.0f;
			Assert.AreEqual(joint.MotorSpeed, 4.0f);
		}

		[Test]
		public void TestLineJointFrequency()
		{
			var physics = Resolve<Physics>();
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateLineJoint(bodyA, bodyB, Point.Zero);
			joint.Frequency = 1.0f;
			Assert.AreEqual(joint.Frequency, 1.0f);
		}

		[Test]
		public void TestLineJointDampingRatio()
		{
			var physics = Resolve<Physics>();
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			physics.CreateLineJoint(bodyA, bodyB, Point.Zero);
		}
	}
}