using NUnit.Framework;

namespace DeltaEngine.Core.Xml.Tests
{
	/// <summary>
	/// Confirms XmlContent can load from file
	/// </summary>
	public class XmlContentTests
	{
		[Test]
		public void LoadXmlContentFromFile()
		{
			using (var xmlContent = new XmlContentFile("TestXml"))
				Assert.AreEqual(37, xmlContent.XmlData.Children.Count);
		}
	}
}