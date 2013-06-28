using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	public class TextWrapperTests : TestWithMocksOrVisually
	{
		[Test]
		public void EmptyText()
		{
			Init();
			var lines = textWrapper.SplitTextIntoLines("", new Size(100, fontData.PixelLineHeight), true);
			Assert.AreEqual(0, lines.Count);
			Window.CloseAfterFrame();
		}

		public void Init()
		{
			fontData = new FontData(ContentLoader.Load<XmlContent>("Verdana12").Data);
			textWrapper = new TextWrapper(fontData.GlyphDictionary, ' ', fontData.PixelLineHeight);
		}

		private FontData fontData;
		private TextWrapper textWrapper;

		[Test]
		public void FontDoesNotFitInTooSmallArea()
		{
			Init();
			var lines = textWrapper.SplitTextIntoLines(ThreeLineText,
				new Size(70, fontData.PixelLineHeight / 2.0f), true);
			Assert.AreEqual(0, lines.Count);
			Window.CloseAfterFrame();
		}

		private const string ThreeLineText = Spaces + "\n" + Spaces + "\n" + Spaces;
		private const string Spaces = "   ";

		[Test]
		public void GetLines()
		{
			Init();
			var lines = textWrapper.SplitTextIntoLines(ThreeLineText,
				new Size(70, fontData.PixelLineHeight * 3), true);
			Assert.AreEqual(3, lines.Count);
			Assert.AreEqual(Spaces, new string(lines[0].ToArray()));
			Assert.AreEqual(Spaces, new string(lines[1].ToArray()));
			Assert.AreEqual(Spaces, new string(lines[2].ToArray()));
			Window.CloseAfterFrame();
		}

		[Test]
		public void ClipTextHeight()
		{
			Init();
			var lines = textWrapper.SplitTextIntoLines(ThreeLineText,
				new Size(60, fontData.PixelLineHeight * 2), true);
			Assert.AreEqual(2, lines.Count);
			Assert.AreEqual(Spaces, new string(lines[0].ToArray()));
			Assert.AreEqual(Spaces, new string(lines[1].ToArray()));
			Window.CloseAfterFrame();
		}

		[Test]
		public void ClipTextWidth()
		{
			Init();
			var lines = textWrapper.SplitTextIntoLines(ThreeLineText,
				new Size(10, fontData.PixelLineHeight * 6), true);
			Assert.AreEqual(6, lines.Count);
			Assert.AreEqual("  ", new string(lines[0].ToArray()));
			Assert.AreEqual(" ", new string(lines[1].ToArray()));
			Assert.AreEqual("  ", new string(lines[2].ToArray()));
			Assert.AreEqual(" ", new string(lines[3].ToArray()));
			Assert.AreEqual("  ", new string(lines[4].ToArray()));
			Assert.AreEqual(" ", new string(lines[5].ToArray()));
			Window.CloseAfterFrame();
		}
	}
}