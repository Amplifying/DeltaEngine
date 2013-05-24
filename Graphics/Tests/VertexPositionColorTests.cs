using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class VertexPositionColorTests
	{
		[Test]
		public void VertexPositionColor2D()
		{
			var vertex = new VertexPositionColor(Point.Zero, Color.Red);
			Assert.AreEqual(vertex.Position, Vector.Zero);
			Assert.AreEqual(vertex.Color, Color.Red);
		}

		[Test]
		public void VertexPositionColor3D()
		{
			var vertex = new VertexPositionColor(Vector.UnitX, Color.Red);
			Assert.AreEqual(vertex.Position, Vector.UnitX);
			Assert.AreEqual(vertex.Color, Color.Red);
		}

		[Test]
		public void VertexPositionColorSizeInBytes()
		{
			Assert.AreEqual(VertexPositionColor.SizeInBytes, 16);
		}
	}
}