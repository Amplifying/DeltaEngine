using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	internal class QuaternionTests
	{
		[Test]
		public void SetQuaternion()
		{
			var quaternion = new Quaternion(5.2f, 2.6f, 4.4f, 1.1f);
			Assert.AreEqual(5.2f, quaternion.X);
			Assert.AreEqual(2.6f, quaternion.Y);
			Assert.AreEqual(4.4f, quaternion.Z);
			Assert.AreEqual(1.1f, quaternion.W);
		}

		[Test]
		public void CreateQuaternionFromAxisAngle()
		{
			var axis = Vector.UnitY;
			const float Angle = 90.0f;
			var quaternion = Quaternion.CreateFromAxisAngle(axis, Angle);
			var sinHalfAngle = MathExtensions.Sin(90 * 0.5f);
			Assert.AreEqual(quaternion.X, axis.X * sinHalfAngle);
			Assert.AreEqual(quaternion.Y, axis.Y * sinHalfAngle);
			Assert.AreEqual(quaternion.Z, axis.Z * sinHalfAngle);
			Assert.AreEqual(quaternion.W, MathExtensions.Cos(90 * 0.5f));
		}

		[Test]
		public void Lerp()
		{
			var rightOrientedQuat = Quaternion.CreateFromAxisAngle(Vector.UnitY, 90.0f);
			var leftOrientedQuat = Quaternion.CreateFromAxisAngle(Vector.UnitY, -90.0f);
			var result = rightOrientedQuat.Lerp(leftOrientedQuat, 0.5f);
			Assert.AreEqual(result, new Quaternion(0.0f, 0.0f, 0.0f, 0.7071f));
		}
	}
}