using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	public class JointTests : TestWithAllFrameworks
	{
		[Test]
		public void TestFixedAngleJoint()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var body = physics.CreateCircle(3.0f);
				var joint = physics.CreateFixedAngleJoint(body, (float)Math.PI / 3);
				Assert.IsNotNull(joint);
			});
		}

		[Test]
		public void TestFixedAngleJointSameBodies()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var body = physics.CreateCircle(3.0f);
				var joint = physics.CreateFixedAngleJoint(body, (float)Math.PI / 3);
				Assert.AreEqual(joint.BodyA, body);
				Assert.AreEqual(joint.BodyB, body);
			});
		}

		[Test]
		public void TestAngleJoint()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var bodyA = physics.CreateCircle(3.0f);
				var bodyB = physics.CreateCircle(3.0f);
				var joint = physics.CreateAngleJoint(bodyA, bodyB, (float)Math.PI / 2);
				Assert.IsNotNull(joint);
			});
		}

		[Test]
		public void TestAngleJointBodiesEqual()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var bodyA = physics.CreateCircle(3.0f);
				var bodyB = physics.CreateCircle(3.0f);
				var joint = physics.CreateAngleJoint(bodyA, bodyB, (float)Math.PI / 2);
				Assert.AreEqual(joint.BodyA, bodyA);
				Assert.AreEqual(joint.BodyB, bodyB);
			});
		}

		[Test]
		public void TestRevoluteJoint()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var bodyA = physics.CreateCircle(3.0f);
				var bodyB = physics.CreateCircle(3.0f);
				var joint = physics.CreateRevoluteJoint(bodyA, bodyB, Point.Zero);
				Assert.IsNotNull(joint);
			});
		}

		[Test]
		public void TestRevoluteJointBodiesEqual()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var bodyA = physics.CreateCircle(3.0f);
				var bodyB = physics.CreateCircle(3.0f);
				var joint = physics.CreateRevoluteJoint(bodyA, bodyB, Point.Zero);
				Assert.AreEqual(joint.BodyA, bodyA);
				Assert.AreEqual(joint.BodyB, bodyB);
			});
		}

		[Test]
		public void TestLineJointMotorEnabled()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var bodyA = physics.CreateCircle(3.0f);
				var bodyB = physics.CreateCircle(3.0f);
				var joint = physics.CreateLineJoint(bodyA, bodyB, Point.Zero);
				Assert.AreEqual(joint.MotorEnabled, false);
				joint.MotorEnabled = true;
				Assert.AreEqual(joint.MotorEnabled, true);
			});
		}

		[Test]
		public void TestLineJointMaxMotorTorque()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var bodyA = physics.CreateCircle(3.0f);
				var bodyB = physics.CreateCircle(3.0f);
				var joint = physics.CreateLineJoint(bodyA, bodyB, Point.Zero);
				joint.MaxMotorTorque = 1.0f;
				Assert.AreEqual(joint.MaxMotorTorque, 1.0f);
			});
		}

		[Test]
		public void TestLineJointMotorSpeed()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var bodyA = physics.CreateCircle(3.0f);
				var bodyB = physics.CreateCircle(3.0f);
				var joint = physics.CreateLineJoint(bodyA, bodyB, Point.Zero);
				joint.MotorSpeed = 4.0f;
				Assert.AreEqual(joint.MotorSpeed, 4.0f);
			});
		}

		[Test]
		public void TestLineJointFrequency()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var bodyA = physics.CreateCircle(3.0f);
				var bodyB = physics.CreateCircle(3.0f);
				var joint = physics.CreateLineJoint(bodyA, bodyB, Point.Zero);
				joint.Frequency = 1.0f;
				Assert.AreEqual(joint.Frequency, 1.0f);
			});
		}

		[Test]
		public void TestLineJointDampingRatio()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var bodyA = physics.CreateCircle(3.0f);
				var bodyB = physics.CreateCircle(3.0f);
				physics.CreateLineJoint(bodyA, bodyB, Point.Zero);
			});
		}
	}
}