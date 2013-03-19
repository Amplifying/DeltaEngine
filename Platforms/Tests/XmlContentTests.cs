using System;
using DeltaEngine.Core;
using DeltaEngine.Core.Xml;
using DeltaEngine.Platforms.Windows;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Confirms XmlContent can load from file
	/// </summary>
	public class XmlContentTests : TestStarter
	{
		[Test]
		public void LoadAndDisposeXmlContentFromFile()
		{
			using (new XmlContentFile("TestXml")) {}
		}

		[IntegrationTest]
		public void LoadXmlContentFromFileUsingContentSystem(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var testXml = content.Load<XmlContent>("TestXml").XmlData;
				Assert.AreEqual(37, testXml.Children.Count);
			});
		}
	}
}