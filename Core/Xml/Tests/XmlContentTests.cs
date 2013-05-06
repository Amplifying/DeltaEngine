using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Content.Disk;
using NUnit.Framework;

namespace DeltaEngine.Core.Xml.Tests
{
	/// <summary>
	/// Confirms XmlContent can load from file
	/// </summary>
	public class XmlContentTests
	{
		[Test, Category("Slow")]
		public void LoadXmlContentFromFile()
		{
			var contentLoader = new DiskContentLoader(new MockXmlContentResolver());
			var xmlContent = contentLoader.Load<XmlContent>("Test");
			Assert.False(xmlContent.IsDisposed);
			Assert.AreEqual("Root", xmlContent.Data.Name);
			Assert.AreEqual(1, xmlContent.Data.Children.Count);
			Assert.AreEqual("Hi", xmlContent.Data.Children[0].Name);
		}

		[Test]
		public void LoadXmlContentFromMock()
		{
			var contentLoader = new MockXmlContentLoader(new MockXmlContentResolver());
			var xmlContent = contentLoader.Load<XmlContent>("Test");
			Assert.False(xmlContent.IsDisposed);
			Assert.AreEqual("Root", xmlContent.Data.Name);
			Assert.AreEqual(1, xmlContent.Data.Children.Count);
			Assert.AreEqual("Hi", xmlContent.Data.Children[0].Name);
			xmlContent.Dispose();
		}

		public class MockXmlContentLoader : ContentLoader
		{
			public MockXmlContentLoader(MockXmlContentResolver resolver)
				: base(resolver) {}

			protected override Stream GetContentDataStream(string contentName)
			{
				var xmlMock = new XmlData("Root");
				xmlMock.AddChild(new XmlData("Hi"));
				return new MemoryStream(StringExtensions.ToByteArray(xmlMock.ToXmlString()));
			}
		}

		public class MockXmlContentResolver : ContentDataResolver
		{
			public ContentData Resolve(Type contentType, string contentName)
			{
				return Activator.CreateInstance(contentType, contentName, null) as ContentData;
			}
		}
	}
}