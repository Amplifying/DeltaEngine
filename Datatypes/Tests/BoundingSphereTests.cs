using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	class BoundingSphereTests
	{
		[Test]
		public void CreateBoundingSphere()
		{
			var boundingSphere = new BoundingSphere(new Vector(0, 0, 0), 2.5f);
			Assert.AreEqual(new Vector(0, 0, 0), boundingSphere.Center);
			Assert.AreEqual(2.5f, boundingSphere.Radius);
		}
	}
}
