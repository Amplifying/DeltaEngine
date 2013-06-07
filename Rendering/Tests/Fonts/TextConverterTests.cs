using System.IO;
using DeltaEngine.Core.Xml;
using DeltaEngine.Rendering.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	public class TextConverterTests
	{
		[SetUp]
		public void Init()
		{
			var fontData = new FontData(new XmlFile(Path.Combine("Content", "Verdana12.xml")).Root);
			textConverter = new TextConverter(fontData.GlyphDictionary, fontData.PixelLineHeight);
		}

		private TextConverter textConverter;

		[Test]
		public void GetGlyphs()
		{
			var glyphs = textConverter.GetRenderableGlyphs("Test");
			Assert.AreEqual(4, glyphs.Length);
		}
	}
}