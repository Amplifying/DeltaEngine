using System.Collections.Generic;
using System.IO;
using DeltaEngine.Core.Xml;

namespace DeltaEngine.Content.Tests
{
	public class MockContentLoader : ContentLoader
	{
		public MockContentLoader(ContentDataResolver resolver)
			: base(resolver)
		{
			contentFilenames.Add("Test", "Test.xml");
			contentFilenames.Add("DeltaEngineLogo", "DeltaEngineLogo.png");
			contentFilenames.Add("DefaultFont_12_16", "DefaultFont_12_16.xml");
			contentFilenames.Add("Verdana12", "Verdana12.xml");
			contentFilenames.Add("Tahoma30", "Tahoma30.xml");
		}

		protected override Stream GetContentDataStream(string contentName)
		{
			return contentFilenames.ContainsKey(contentName)
				? new XmlFile(new XmlData("Root").AddChild(new XmlData("Hi"))).ToMemoryStream()
				: Stream.Null;
		}

		public override List<Content> LoadRecursively<Content>(string parentName)
		{
			var firstImage = Load<Content>("DeltaEngineLogo");
			var loadedElements = new List<Content>();
			loadedElements.Add(firstImage);
			return loadedElements;
		}
	}
}