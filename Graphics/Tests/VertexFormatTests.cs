using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class VertexFormatTests
	{
		[Test]
		public void VertexElementPosition3D()
		{
			var element = new VertexElement(VertexElementType.Position3D);
			Assert.AreEqual(VertexElementType.Position3D, element.ElementType);
			Assert.AreEqual(3, element.ComponentCount);
			Assert.AreEqual(12, element.Size);
		}

		[Test]
		public void VertexElementTextureUV()
		{
			var element = new VertexElement(VertexElementType.TextureUV);
			Assert.AreEqual(VertexElementType.TextureUV, element.ElementType);
			Assert.AreEqual(2, element.ComponentCount);
			Assert.AreEqual(8, element.Size);
		}

		[Test]
		public void VertexElementColor()
		{
			var element = new VertexElement(VertexElementType.Color);
			Assert.AreEqual(VertexElementType.Color, element.ElementType);
			Assert.AreEqual(4, element.ComponentCount);
			Assert.AreEqual(4, element.Size);
		}

		[Test]
		public void VertexFormatPosition3DTextureUVColor()
		{
			var elements = new[] {
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.TextureUV),
				new VertexElement(VertexElementType.Color)};
			var format = new VertexFormat(elements);
			Assert.AreEqual(24, format.Stride);
			Assert.AreEqual(0, elements[0].Offset);
			Assert.AreEqual(12, elements[1].Offset);
			Assert.AreEqual(20, elements[2].Offset);
		}

		[Test]
		public void VertexFormatGetVertexElement()
		{
			var elements = new[] {
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.TextureUV) };
			var format = new VertexFormat(elements);
			Assert.IsNull(format.GetElementFromType(VertexElementType.Color));
			Assert.IsNotNull(format.GetElementFromType(VertexElementType.TextureUV));
		}
	}
}
