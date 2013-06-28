using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	internal class QuaternionTests
	{
		[Test]
		public void CreateQuaternion()
		{
			var quaternion = new Quaternion(5.2f, 2.6f, 4.4f, 1.1f);
			Assert.AreEqual(5.2f, quaternion.X);
			Assert.AreEqual(2.6f, quaternion.Y);
			Assert.AreEqual(4.4f, quaternion.Z);
			Assert.AreEqual(1.1f, quaternion.W);
		}
	}
}