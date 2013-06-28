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
			var polygon = new Polygon(Rectangle.One, Color.White);
			polygon.Points.AddRange(new[] { Point.Zero, Point.One, Point.UnitY });
			Assert.AreEqual(Rectangle.One, polygon.DrawArea);
			Assert.AreEqual(Color.White, polygon.Color);
		}

		[Test]
		public void ChangeOutlineColor()
		{
			var polygon = new Polygon(Rectangle.One, Color.Red);
			polygon.Add(new OutlineColor(Color.Blue)).Start<Polygon.RenderOutline>();
			Assert.AreEqual(Color.Blue, polygon.Get<OutlineColor>().Value);
		}
	}
}