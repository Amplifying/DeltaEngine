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
			Assert.AreEqual(Rectangle.Zero, polygon.DrawArea);
		}

		[Test]
		public void ChangeOutlineColor()
		{
			var polygon =
				new Polygon(Color.Red).Add(new OutlineColor(Color.Blue)).Add<Polygon.RenderOutline>();
			Assert.AreEqual(Color.Blue, polygon.Get<OutlineColor>().Value);
		}
	}
}