using System;
using System.IO;
using DeltaEngine.Core.Xml;
using DeltaEngine.Rendering.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	public class TextParserTests
	{
		[SetUp]
		public void Init()
		{
			var fontData = new FontData(new XmlFile(Path.Combine("Content", "Verdana12.xml")).Root);
			textParser = new TextParser(fontData.GlyphDictionary, '?');
		}

		private TextParser textParser;

		[Test]
		public void ParseEmptyText()
		{
			var lines = textParser.GetLines("");
			Assert.AreEqual(0, lines.Count);
		}

		[Test]
		public void ParseSingleTextLine()
		{
			var lines = textParser.GetLines("long loong looong loooong text");
			Assert.AreEqual(1, lines.Count);
			Assert.AreEqual("long loong looong loooong text", new string(lines[0].ToArray()));
		}

		[Test]
		public void ParseMultipleTextLines()
		{
			var lines = textParser.GetLines("first text line" + Environment.NewLine + "Newline");
			Assert.AreEqual(2, lines.Count);
			Assert.AreEqual("first text line", new string(lines[0].ToArray()));
			Assert.AreEqual("Newline", new string(lines[1].ToArray()));
		}

		[Test]
		public void ConvertTabsIntoTwoSpaces()
		{
			var lines = textParser.GetLines("\ttab\t");
			Assert.AreEqual(1, lines.Count);
			Assert.AreEqual("  tab  ", new string(lines[0].ToArray()));
		}

		[Test]
		public void ParseWithUnsupportedCharacters()
		{
			var lines = textParser.GetLines("€TextäöüTextÄÖÜ");
			Assert.AreEqual(1, lines.Count);
			Assert.AreEqual("?Text???Text???", new string(lines[0].ToArray()));
		}
	}
}