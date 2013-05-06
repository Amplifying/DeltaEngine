using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class VertexPositionColorTexturedTests
	{
		[Test]
		public void VertexPositionColorTextured2D()
		{
			var vertex = new VertexPositionColorTextured(Point.Zero, Color.Red, Point.One);
			Assert.AreEqual(vertex.Position, Vector.Zero);
			Assert.AreEqual(vertex.Color, Color.Red);
			Assert.AreEqual(vertex.TextureCoordinate, Point.One);
		}

		[Test]
		public void VertexPositionColorTextured3D()
		{
			var vertex = new VertexPositionColorTextured(Vector.UnitX, Color.Red, Point.One);
			Assert.AreEqual(vertex.Position, Vector.UnitX);
			Assert.AreEqual(vertex.Color, Color.Red);
			Assert.AreEqual(vertex.TextureCoordinate, Point.One);
		}

		[Test]
		public void VertexPositionColorTexturedSizeInBytes()
		{
			Assert.AreEqual(VertexPositionColorTextured.SizeInBytes, 24);
		}
	}
}