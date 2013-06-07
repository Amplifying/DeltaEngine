using System.IO;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	public class FontDataTests
	{
		[Test]
		public void LoadVerdanaFontData()
		{
			var fontData = new FontData(new XmlFile(Path.Combine("Content", "Verdana12.xml")).Root);
			Assert.AreEqual("Verdana", fontData.FontFamilyName);
			Assert.AreEqual(12, fontData.SizeInPoints);
			Assert.AreEqual("AddOutline", fontData.Style);
			Assert.AreEqual(16, fontData.PixelLineHeight);
			Assert.AreEqual("Verdana12Font", fontData.FontMapName);
			Assert.AreEqual(new Size(128, 128), fontData.FontMapPixelSize);
			Assert.AreEqual(95, fontData.GlyphDictionary.Count);
			Assert.AreEqual(new Rectangle(0, 0, 1, 16), fontData.GlyphDictionary[' '].UV);
			Assert.AreEqual(7.34875f, fontData.GlyphDictionary[' '].AdvanceWidth);
		}

		[Test]
		public void LoadTahomaFontData()
		{
			var fontData = new FontData(new XmlFile(Path.Combine("Content", "Tahoma30.xml")).Root);
			Assert.AreEqual("Tahoma", fontData.FontFamilyName);
			Assert.AreEqual(30, fontData.SizeInPoints);
			Assert.AreEqual("Bold", fontData.Style);
			Assert.AreEqual(50, fontData.PixelLineHeight);
			Assert.AreEqual("Tahoma30Font", fontData.FontMapName);
			Assert.AreEqual(new Size(512, 512), fontData.FontMapPixelSize);
			Assert.AreEqual(95, fontData.GlyphDictionary.Count);
			Assert.AreEqual(new Rectangle(0, 1, 4, 50), fontData.GlyphDictionary[' '].UV);
			Assert.AreEqual(20.1449f, fontData.GlyphDictionary[' '].AdvanceWidth);
		}

		[Test]
		public void LoadFromInvalidXmlDataThrows()
		{
			Assert.Throws<FontData.UnableToLoadFontDataWithoutValidXmlData>(() => new FontData(null));
			Assert.Throws<FontData.UnableToLoadFontDataWithoutValidXmlData>(
				() => new FontData(new XmlData("invalid")));
		}

		[Test]
		public void GetGlyphDrawAreaAndUVs()
		{
			var fontData = new FontData(new XmlFile(Path.Combine("Content", "Verdana12.xml")).Root);
			fontData.Generate("");
			Assert.AreEqual(0, fontData.Glyphs.Length);
			fontData.Generate("\n");
			Assert.AreEqual(0, fontData.Glyphs.Length);
			fontData.Generate("A");
			Assert.AreEqual(1, fontData.Glyphs.Length);
			GlyphDrawData glyphA = fontData.Glyphs[0];
			Assert.AreEqual(glyphA.UV,
				Rectangle.BuildUvRectangle(new Rectangle(67, 32, 9, 16), new Size(128, 128)));
			Assert.AreEqual(new Rectangle(0, 0, 9, 16), glyphA.DrawArea);
		}

		[Test]
		public void GetGlyphsForMultilineText()
		{
			var fontData = new FontData(new XmlFile(Path.Combine("Content", "Verdana12.xml")).Root);
			fontData.Generate("Abc\ndef\nbrabbel brabbel");
			Assert.AreEqual(21, fontData.Glyphs.Length);
		}

		[Test]
		public void CreationWithInvalidXmlDataShouldThrow()
		{
			var xmlFile = new XmlFile(Path.Combine("Content", "Tahoma30.xml")).Root;
			foreach (var xmlData in xmlFile.GetChild("Kernings").GetChildren("Kerning"))
				xmlData.RemoveAttribute("First");

			Assert.Throws<InvalidDataException>(() => new FontData(xmlFile));
		}

		[Test]
		public void ToStringTest()
		{
			var type = typeof(FontData);
			var expected = type.Namespace + "." + type.Name + ", Font Family=Verdana, Font Size=12";
			var fontData = new FontData(new XmlFile(Path.Combine("Content", "Verdana12.xml")).Root);
			Assert.AreEqual(expected, fontData.ToString());
		}
	}
}