using System;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	public class TextParserTests : TestWithMocksOrVisually
	{
		[Test]
		public void ParseEmptyText()
		{
			var lines = GetTextParser().GetLines(GetSpaces(0));
			Assert.AreEqual(0, lines.Count);
			Window.CloseAfterFrame();
		}

		public TextParser GetTextParser()
		{
			var fontData = new FontData(ContentLoader.Load<XmlContent>("Verdana12").Data);
			return new TextParser(fontData.GlyphDictionary, ' ');
		}

		private static string GetSpaces(int numberOfSpaces)
		{
			string spaces = "";
			for (int i = 0; i < numberOfSpaces; i++)
				spaces += " ";

			return spaces;
		}

		[Test]
		public void ParseSingleTextLine()
		{
			var lines = GetTextParser().GetLines(GetSpaces(3));
			Assert.AreEqual(1, lines.Count);
			Assert.AreEqual(GetSpaces(3), new string(lines[0].ToArray()));
			Window.CloseAfterFrame();
		}

		[Test]
		public void ParseMultipleTextLines()
		{
			var lines = GetTextParser().GetLines(GetSpaces(1) + Environment.NewLine + GetSpaces(2));
			Assert.AreEqual(2, lines.Count);
			Assert.AreEqual(GetSpaces(1), new string(lines[0].ToArray()));
			Assert.AreEqual(GetSpaces(2), new string(lines[1].ToArray()));
			Window.CloseAfterFrame();
		}

		[Test]
		public void ConvertTabsIntoTwoSpaces()
		{
			var lines = GetTextParser().GetLines("\t \t");
			Assert.AreEqual(1, lines.Count);
			Assert.AreEqual(GetSpaces(5), new string(lines[0].ToArray()));
			Window.CloseAfterFrame();
		}

		[Test]
		public void ParseWithUnsupportedCharacters()
		{
			var lines = GetTextParser().GetLines("äöüÄÖÜ");
			Assert.AreEqual(1, lines.Count);
			Assert.AreEqual(GetSpaces(6), new string(lines[0].ToArray()));
			Window.CloseAfterFrame();
		}
	}
}