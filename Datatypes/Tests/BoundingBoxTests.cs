using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	class BoundingBoxTests
	{
		[Test]
		public void CreateBoundingBox()
		{
			var boundingBox = new BoundingBox(new Vector(0, 0, 0), new Vector(1, 1, 1));
			Assert.AreEqual(new Vector(0, 0, 0), boundingBox.Min);
			Assert.AreEqual(new Vector(1, 1, 1), boundingBox.Max);
		}
	}
}
