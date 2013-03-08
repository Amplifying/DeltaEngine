using System;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Core.Xml.Tests
{
	/// <summary>
	/// Confirms XmlContent can load from file
	/// </summary>
	public class XmlContentTests : TestStarter
	{
		[IntegrationTest]
		public void LoadExistingContentFromFile(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var testXml = content.Load<XmlContent>("Test");
				Assert.AreEqual(37, testXml.XmlData.Children.Count);
			});
		}
	}
}