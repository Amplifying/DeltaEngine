using System.IO;
using DeltaEngine.Content.Xml;

namespace DeltaEngine.Content.Tests
{
	public class MockContentLoader : ContentLoader
	{
		public MockContentLoader(ContentDataResolver resolver)
			: base(resolver, "Content") {}

		protected override void LazyInitialize() {}

		protected override ContentMetaData GetMetaData(string contentName)
		{
			return new ContentMetaData(contentName, ContentType.Xml);
		}

		protected override Stream GetContentDataStream(ContentData content)
		{
			var stream = Stream.Null;
			if (content.Name.Equals("Test"))
				stream = new XmlFile(new XmlData("Root").AddChild(new XmlData("Hi"))).ToMemoryStream();

			return stream;
		}
	}
}