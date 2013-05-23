using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class PolygonTests
	{
		[Test]
		public void NewPolygon()
		{
			var polygon = new Polygon();
			Assert.AreEqual(Color.White, polygon.Color);
			Assert.AreEqual(Color.White, polygon.OutlineColor);
			Assert.AreEqual(Rectangle.Zero, polygon.DrawArea);
		}

		[Test]
		public void ChangeOutlineColor()
		{
			var polygon = new Polygon(Color.Red) { OutlineColor = Color.Blue };
			Assert.AreEqual(Color.Blue, polygon.OutlineColor);
		}
	}
}