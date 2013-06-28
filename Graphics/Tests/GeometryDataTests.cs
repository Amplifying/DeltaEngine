using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class GeometryDataTests
	{
		[Test]
		public void CreateGeometryDataShouldBeEmpty()
		{
			var geometryData = new GeometryData(CreateVertexFormat(), 100, 300);
			Assert.AreEqual(0, geometryData.NumberOfVertices);
			Assert.AreEqual(0, geometryData.NumberOfIndices);
		}

		[Test]
		public void AddGeometryData()
		{
			var geometryData = new GeometryData(CreateVertexFormat(),	2, 0);
			geometryData.AddVertexData(Vector.Zero, Point.Zero, Color.White);
			geometryData.AddVertexData(Vector.One, Point.One, Color.Red);
			Assert.AreEqual(2, geometryData.NumberOfVertices);
			Assert.AreEqual(0, geometryData.NumberOfIndices);
		}

		[Test]
		public void AddGeometryIndices()
		{
			var geometryData = new GeometryData(CreateVertexFormat(), 0, 5);
			geometryData.AddIndices(new[] { 0, 1, 2, 3, 4 });
			Assert.AreEqual(5, geometryData.NumberOfIndices);
		}

		private static VertexFormat CreateVertexFormat()
		{
			return new VertexFormat(new[]
			{
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.TextureUV),
				new VertexElement(VertexElementType.Color)
			});
		}
	}
}