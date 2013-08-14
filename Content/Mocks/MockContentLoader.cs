using System;
using System.IO;
using DeltaEngine.Content.Xml;
using DeltaEngine.Extensions;

namespace DeltaEngine.Content.Mocks
{
	/// <summary>
	/// Loads mock content used in unit tests.
	/// </summary>
	public class MockContentLoader : ContentLoader
	{
		public MockContentLoader(ContentDataResolver resolver)
			: base("Content")
		{
			base.resolver = resolver;
		}

		protected override Stream GetContentDataStream(ContentData content)
		{
			var stream = Stream.Null;
			if (content.Name.Equals("Test"))
				stream = new XmlFile(new XmlData("Root").AddChild(new XmlData("Hi"))).ToMemoryStream();
			else if (content.Name.Equals("Texts"))
				stream = new XmlFile(new XmlData("Texts").AddChild(GoLocalizationNode)).ToMemoryStream();
			else if (content.Name.Equals("DefaultCommands"))
				stream = CreateInputCommandXml().ToMemoryStream();
			else if (content.Name.Equals("Level"))
				stream = new MemoryStream(LoadLevelJsonAsBytes());
			else if (content.Name.Equals("Verdana12") || content.Name.Equals("Tahoma30"))
				stream = CreateFontXml().ToMemoryStream();
			return stream;
		}

		protected override bool HasValidContentAndMakeSureItIsLoaded()
		{
			return true;
		}

		private static byte[] LoadLevelJsonAsBytes()
		{
			return StringExtensions.ToByteArray(@"""objects"":[
	{
		""width"":32,
		""height"":32,
		""name"":""player""
	}, 
	{
		""width"":32,
		""height"":32,
		""name"":""monster""
	},");
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

		private static XmlData GoLocalizationNode
		{
			get
			{
				var keyNode = new XmlData("Go");
				keyNode.AddAttribute("en", "Go");
				keyNode.AddAttribute("de", "Los");
				keyNode.AddAttribute("es", "¡vamos!");
				return keyNode;
			}
		}

		private static XmlFile CreateInputCommandXml()
		{
			var inputSettings = new XmlData("InputSettings");
			var exit = new XmlData("Command").AddAttribute("Name", "Exit");
			exit.AddChild("KeyTrigger", "Escape");
			inputSettings.AddChild(exit);
			var click = new XmlData("Command").AddAttribute("Name", "Click");
			click.AddChild("KeyTrigger", "Space");
			click.AddChild("MouseButtonTrigger", "Left");
			click.AddChild("TouchPressTrigger", "");
			click.AddChild("GamePadButtonTrigger", "A");
			inputSettings.AddChild(click);
			var moveLeft = new XmlData("Command").AddAttribute("Name", "MoveLeft");
			moveLeft.AddChild("KeyTrigger", "CursorLeft Pressed");
			moveLeft.AddChild("KeyTrigger", "A");
			moveLeft.AddChild("GamePadAnalogTrigger", "LeftThumbStick");
			inputSettings.AddChild(moveLeft);
			var moveRight = new XmlData("Command").AddAttribute("Name", "MoveRight");
			moveRight.AddChild("KeyTrigger", "CursorRight Pressed");
			moveRight.AddChild("KeyTrigger", "D");
			moveRight.AddChild("GamePadAnalogTrigger", "RightThumbStick");
			inputSettings.AddChild(moveRight);
			var moveDirectly = new XmlData("Command").AddAttribute("Name", "MoveDirectly");
			moveDirectly.AddChild("MousePositionTrigger", "Left");
			moveDirectly.AddChild(new XmlData("TouchPressTrigger"));
			inputSettings.AddChild(moveDirectly);
			return new XmlFile(inputSettings);
		}

		protected override ContentMetaData GetMetaData(string contentName, Type contentClassType = null)
		{
			if (contentName.Equals("UnavailableImage"))
				return null;
			ContentType contentType = ConvertClassTypeToContentType(contentClassType);
			if (contentType == ContentType.Material)
				return CreateMaterialMetaData(contentName);
			if (contentName.Contains("SpriteSheet") || contentType == ContentType.SpriteSheetAnimation)
				return CreateSpriteSheetAnimationMetaData(contentName);
			if (contentName == "ImageAnimation" || contentType == ContentType.ImageAnimation)
				return CreateImageAnimationMetaData(contentName);
			if (contentType == ContentType.Image)
				return CreateImageMetaData(contentName);
			if (contentType == ContentType.Shader)
				return null;
			return new ContentMetaData { Name = contentName, Type = contentType };
		}

		private static ContentType ConvertClassTypeToContentType(Type contentClassType)
		{
			if (contentClassType == null)
				return ContentType.Xml;
			var typeName = contentClassType.Name;
			foreach (var contentType in EnumExtensions.GetEnumValues<ContentType>())
				if (contentType != ContentType.Image && contentType != ContentType.Mesh &&
					typeName.Contains(contentType.ToString()))
					return contentType;
			if (typeName.Contains("Image") || typeName.Contains("Texture"))
				return ContentType.Image;
			if (typeName.Contains("Mesh") || typeName.Contains("Geometry"))
				return ContentType.Mesh;
			return ContentType.Xml;
		}

		private static ContentMetaData CreateMaterialMetaData(string name)
		{
			var metaData = new ContentMetaData { Name = name, Type = ContentType.Material };
			metaData.Values.Add("ShaderName", "Position2DUv");
			metaData.Values.Add("ImageOrAnimationName", "DeltaEngineLogo");
			return metaData;
		}

		private static ContentMetaData CreateSpriteSheetAnimationMetaData(string name)
		{
			var metaData = new ContentMetaData { Name = name, Type = ContentType.SpriteSheetAnimation };
			metaData.Values.Add("ImageName", "EarthImages");
			metaData.Values.Add("DefaultDuration", "5.0");
			metaData.Values.Add("SubImageSize", "32,32");
			return metaData;
		}

		private static ContentMetaData CreateImageAnimationMetaData(string name)
		{
			var metaData = new ContentMetaData { Name = name, Type = ContentType.ImageAnimation };
			metaData.Values.Add("ImageNames", "ImageAnimation01,ImageAnimation02,ImageAnimation03");
			metaData.Values.Add("DefaultDuration", "3");
			return metaData;
		}

		private static ContentMetaData CreateImageMetaData(string contentName)
		{
			var metaData = new ContentMetaData { Name = contentName, Type = ContentType.Image };
			metaData.Values.Add("PixelSize", "128,128");
			return metaData;
		}
	}
}