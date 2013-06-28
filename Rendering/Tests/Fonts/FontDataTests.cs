using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	public class FontDataTests : TestWithMocksOrVisually
	{
		[Test]
		public void LoadFontData()
		{
			var fontData = new FontData(ContentLoader.Load<XmlContent>("Verdana12").Data);
			Assert.AreEqual("Verdana", fontData.FontFamilyName);
			Assert.AreEqual(12, fontData.SizeInPoints);
			Assert.AreEqual("AddOutline", fontData.Style);
			Assert.AreEqual(16, fontData.PixelLineHeight);
			Assert.AreEqual("Verdana12Font", fontData.FontMapName);
			Assert.AreEqual(new Size(128, 128), fontData.FontMapPixelSize);
			//Assert.AreEqual(95, fontData.GlyphDictionary.Count);
			Assert.AreEqual(new Rectangle(0, 0, 1, 16), fontData.GlyphDictionary[' '].UV);
			Assert.AreEqual(7.34875f, fontData.GlyphDictionary[' '].AdvanceWidth);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LoadFromInvalidXmlDataThrows()
		{
			Assert.Throws<FontData.UnableToLoadFontDataWithoutValidXmlData>(() => new FontData(null));
			Window.CloseAfterFrame();
		}

		[Test]
		public void GetGlyphDrawAreaAndUVs()
		{
			var fontData = new FontData(ContentLoader.Load<XmlContent>("Verdana12").Data);
			fontData.Generate("");
			Assert.AreEqual(0, fontData.Glyphs.Length);
			fontData.Generate("\n");
			Assert.AreEqual(0, fontData.Glyphs.Length);
			fontData.Generate(" ");
			Assert.AreEqual(1, fontData.Glyphs.Length);
			GlyphDrawData glyphA = fontData.Glyphs[0];
			Assert.AreEqual(glyphA.UV,
				Rectangle.BuildUvRectangle(new Rectangle(0, 0, 1, 16), new Size(128, 128)));
			Assert.AreEqual(new Rectangle(0, 0, 1, 16), glyphA.DrawArea);
			Window.CloseAfterFrame();
		}

		[Test]
		public void GetGlyphsForMultilineText()
		{
			var fontData = new FontData(ContentLoader.Load<XmlContent>("Verdana12").Data);
			fontData.Generate(" \n \n ");
			Assert.AreEqual(3, fontData.Glyphs.Length);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ToStringTest()
		{
			var fontDataType = typeof(FontData);
			var expected = fontDataType.Namespace + "." + fontDataType.Name +
				", Font Family=Verdana, Font Size=12";
			var fontData = new FontData(ContentLoader.Load<XmlContent>("Verdana12").Data);
			Assert.AreEqual(expected, fontData.ToString());
			Window.CloseAfterFrame();
		}
	}
}