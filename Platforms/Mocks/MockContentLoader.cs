using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Content.Json;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;

namespace DeltaEngine.Platforms.Mocks
{
	internal class MockContentLoader : ContentLoader
	{
		public MockContentLoader(ContentDataResolver resolver)
			: base(resolver, "Content") {}

		protected override void LazyInitialize() {}

		protected override Stream GetContentDataStream(ContentData content)
		{
			var stream = Stream.Null;
			if (content.Name.Equals("Test"))
				stream = new XmlFile(new XmlData("Root").AddChild(new XmlData("Hi"))).ToMemoryStream();
			else if (content.Name.Equals("Level"))
				stream = new MemoryStream(LoadLevelJsonAsBytes());
			else if (content.Name.Equals("Verdana12") || content.Name.Equals("Tahoma30"))
				stream = CreateFontXml().ToMemoryStream();

			return stream;
		}

		private static byte[] LoadLevelJsonAsBytes()
		{
			var jsonData = new JsonFile(@"c:\code\DeltaEngine\ContentCache\TinyPlatformer\Level.json");
			return StringExtensions.ToByteArray(jsonData.Root.ToString());
		}

		private static XmlFile CreateFontXml()
		{
			var glyph = new XmlData("Glyph");
			glyph.AddAttribute("Character", ' ');
			glyph.AddAttribute("UV", "0 0 1 16");
			glyph.AddAttribute("AdvanceWidth", "7.34875");
			glyph.AddAttribute("LeftBearing", "0");
			glyph.AddAttribute("RightBearing", "4.21875");
			var glyphs = new XmlData("Glyphs").AddChild(glyph);
			var bitmap = new XmlData("Bitmap");
			bitmap.AddAttribute("Name", "Verdana12Font");
			bitmap.AddAttribute("Width", "128");
			bitmap.AddAttribute("Height", "128");
			var font = new XmlData("Font");
			font.AddAttribute("Family", "Verdana");
			font.AddAttribute("Size", "12");
			font.AddAttribute("Style", "AddOutline");
			font.AddAttribute("LineHeight", "16");
			font.AddChild(bitmap).AddChild(glyphs);
			return new XmlFile(font);
		}

		protected override ContentMetaData GetMetaData(string contentName)
		{
			if (contentName.Equals("UnavailableImage"))
				throw new ContentNotFound(contentName);
			if (contentName.Equals("ImageAnimation") || contentName.Contains("TowerIdle") ||
				contentName.Contains("TowerAttack") || contentName.Contains("CreepWalk") ||
				contentName.Contains("CreepDie"))
				return CreateImageAnimationMetaData();
			if (contentName.Equals("SpriteSheetAnimation"))
				return CreateSpriteSheetAnimationMetaData();
			if (contentName.Equals("DeltaEngineLogo"))
				return CreateImageMetaData(contentName);
			return new ContentMetaData(contentName, ContentType.Xml);
		}

		private static ContentMetaData CreateImageMetaData(string contentName)
		{
			var metaData = new ContentMetaData(contentName, ContentType.Image);
			metaData.Values.Add("PixelSize", "128,128");
			return metaData;
		}

		private static ContentMetaData CreateImageAnimationMetaData()
		{
			var metaData = new ContentMetaData("ImageAnimation", ContentType.ImageAnimation);
			metaData.Values.Add("Duration", "3");
			metaData.AddChildren(new ContentMetaData("ImageAnimation01", ContentType.Image));
			metaData.AddChildren(new ContentMetaData("ImageAnimation02", ContentType.Image));
			metaData.AddChildren(new ContentMetaData("ImageAnimation03", ContentType.Image));
			return metaData;
		}

		private static ContentMetaData CreateSpriteSheetAnimationMetaData()
		{
			var metaData = new ContentMetaData("SpriteSheetAnimation", ContentType.ImageAnimation);
			metaData.Values.Add("Duration", "5.0");
			metaData.Values.Add("SubImageSize", "32,32");
			var child = new ContentMetaData("WalkingCharacter", ContentType.Image);
			metaData.Values.Add("PixelSize", "160,128");
			metaData.AddChildren(child);
			return metaData;
		}
	}
}