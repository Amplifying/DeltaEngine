using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes3D;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes3D
{
	public class BoxTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateRedBox()
		{
			var box = new Box(Vector.One, Color.Red);
			Assert.AreEqual(8, box.Get<Geometry>().NumberOfVertices);
			Assert.AreEqual(36, box.Get<Geometry>().NumberOfIndices);
			Assert.AreEqual(Color.Red, box.Get<Color>());
		}
	}
}