using System.IO;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	public class TextWrapperTests
	{
		[SetUp]
		public void Init()
		{
			fontData = new FontData(new XmlFile(Path.Combine("Content", "Verdana12.xml")).Root);
			textWrapper = new TextWrapper(fontData.GlyphDictionary, '?', fontData.PixelLineHeight);
		}

		private FontData fontData;
		private TextWrapper textWrapper;

		[Test]
		public void EmptyText()
		{
			var lines = textWrapper.SplitTextIntoLines("", new Size(100, fontData.PixelLineHeight),
				true);
			Assert.AreEqual(0, lines.Count);
		}

		[Test]
		public void FontDoesNotFitInTooSmallArea()
		{
			var lines = textWrapper.SplitTextIntoLines(ThreeLineText,
				new Size(70, fontData.PixelLineHeight / 2.0f), true);
			Assert.AreEqual(0, lines.Count);
		}

		[Test]
		public void GetLines()
		{
			var lines = textWrapper.SplitTextIntoLines(ThreeLineText,
				new Size(70, fontData.PixelLineHeight * 3), true);
			Assert.AreEqual(3, lines.Count);
			Assert.AreEqual("Line one", new string(lines[0].ToArray()));
			Assert.AreEqual("Line two", new string(lines[1].ToArray()));
			Assert.AreEqual("Line three", new string(lines[2].ToArray()));
		}

		private const string ThreeLineText = "Line one\nLine two\nLine three";

		[Test]
		public void ClipTextHeight()
		{
			var lines = textWrapper.SplitTextIntoLines(ThreeLineText,
				new Size(60, fontData.PixelLineHeight * 2), true);
			Assert.AreEqual(2, lines.Count);
			Assert.AreEqual("Line one", new string(lines[0].ToArray()));
			Assert.AreEqual("Line two", new string(lines[1].ToArray()));
		}

		[Test]
		public void ClipTextWidth()
		{
			var lines = textWrapper.SplitTextIntoLines(ThreeLineText,
				new Size(50, fontData.PixelLineHeight * 6), true);
			Assert.AreEqual(6, lines.Count);
			Assert.AreEqual("Line ", new string(lines[0].ToArray()));
			Assert.AreEqual("one", new string(lines[1].ToArray()));
			Assert.AreEqual("Line ", new string(lines[2].ToArray()));
			Assert.AreEqual("two", new string(lines[3].ToArray()));
			Assert.AreEqual("Line ", new string(lines[4].ToArray()));
			Assert.AreEqual("three", new string(lines[5].ToArray()));
		}
	}
}