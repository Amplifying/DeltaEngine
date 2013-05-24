using System;
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
			Assert.AreEqual("Verdana12Font", fontData.FontMapName);
			Assert.AreEqual(new Size(128, 128), fontData.FontMapPixelSize);
			Assert.AreEqual(95, fontData.glyphDictionary.Count);
			Assert.AreEqual(new Rectangle(0, 0, 1, 16), fontData.glyphDictionary[' '].UV);
			Assert.AreEqual(7.34875f, fontData.glyphDictionary[' '].AdvanceWidth);
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
			Assert.AreEqual(95, fontData.glyphDictionary.Count);
			Assert.AreEqual(new Rectangle(0, 1, 4, 50), fontData.glyphDictionary[' '].UV);
			Assert.AreEqual(20.1449f, fontData.glyphDictionary[' '].AdvanceWidth);
		}

		[Test]
		public void ParseTextLines()
		{
			var fontData = new FontData(new XmlFile(Path.Combine("Content", "Verdana12.xml")).Root);
			var lines = fontData.ParseText("long loong looong loooong text");
			Assert.NotNull(lines);
			Assert.AreEqual(lines.Count, 1);
			Assert.AreEqual(new string(lines[0].ToArray()), "long loong looong loooong text");

			lines = fontData.ParseText("long loong looong looong text" + Environment.NewLine + "Newline");
			Assert.NotNull(lines);
			Assert.AreEqual(lines.Count, 2);
			Assert.AreEqual(new string(lines[0].ToArray()), "long loong looong looong text");
			Assert.AreEqual(new string(lines[1].ToArray()), "Newline");
		}

		[Test]
		public void GetGlyphDrawAreaAndUVs()
		{
			var fontData = new FontData(new XmlFile(Path.Combine("Content", "Verdana12.xml")).Root);
			const HorizontalAlignment Alignment = HorizontalAlignment.Left;
			var drawSize = Size.Zero;
			var glyphs = fontData.GetGlyphDrawAreaAndUVs("", 1.0f, Alignment, false, ref drawSize);
			Assert.AreEqual(0, glyphs.Length);
			drawSize = Size.Zero;
			glyphs = fontData.GetGlyphDrawAreaAndUVs("\n", 1.0f, Alignment, false, ref drawSize);
			Assert.AreEqual(0, glyphs.Length);

			drawSize = Size.Zero;
			glyphs = fontData.GetGlyphDrawAreaAndUVs("A", 1.0f, Alignment, false, ref drawSize);
			Assert.AreEqual(1, glyphs.Length);
			GlyphDrawAreaAndUV glyphA = glyphs[0];
			Assert.AreEqual(glyphA.UV,
				Rectangle.BuildUVRectangle(new Rectangle(67, 32, 9, 16), new Size(128, 128)));
			Assert.AreEqual(new Rectangle(0, 0, 9, 16), glyphA.DrawArea);
		}

		[Test]
		public void GetGlyphsForMultilineText()
		{
			var fontData = new FontData(new XmlFile(Path.Combine("Content", "Verdana12.xml")).Root);
			var drawSize = new Size(50, 100);
			var glyphs = fontData.GetGlyphDrawAreaAndUVs("Abc\ndef\nbrabbel brabbel", 1.0f,
				HorizontalAlignment.Left, true, ref drawSize);
			Assert.AreEqual(21, glyphs.Length);
		}
	}
}