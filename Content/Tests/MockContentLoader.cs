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
			contentFilenames.Add("ImageAnimation01", "ImageAnimation01.png");
			contentFilenames.Add("ImageAnimation02", "ImageAnimation02.png");
			contentFilenames.Add("ImageAnimation03", "ImageAnimation03.png");
		}

		protected override Stream GetContentDataStream(string contentName)
		{
			return contentFilenames.ContainsKey(contentName)
				? new XmlFile(new XmlData("Root").AddChild(new XmlData("Hi"))).ToMemoryStream()
				: Stream.Null;
		}

		public override List<Content> LoadRecursively<Content>(string parentName)
		{
			var loadedElements = new List<Content>();
			if (parentName == "ImageAnimation")
			{
				loadedElements.Add(Load<Content>("ImageAnimation01"));
				loadedElements.Add(Load<Content>("ImageAnimation02"));
				loadedElements.Add(Load<Content>("ImageAnimation03"));
			}
			return loadedElements;
		}
	}
}