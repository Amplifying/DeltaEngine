using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	public class TextConverterTests : TestWithMocksOrVisually
	{
		[Test]
		public void GetGlyphs()
		{
			var fontData = new FontData(ContentLoader.Load<XmlContent>("Verdana12").Data);
			var textConverter = new TextConverter(fontData.GlyphDictionary, fontData.PixelLineHeight);
			var glyphs = textConverter.GetRenderableGlyphs("    ");
			Assert.AreEqual(4, glyphs.Length);
			Window.CloseAfterFrame();
		}
	}
}