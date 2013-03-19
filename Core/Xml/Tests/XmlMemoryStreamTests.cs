using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Core.Xml.Tests
{
	public class XmlMemoryStreamTests
	{
		[Test]
		public void XmlDataConstructor()
		{
			var data = new XmlData("name");
			var file = new XmlMemoryStream(data);
			Assert.AreEqual(data, file.Root);
		}

		[Test]
		public void SavingAndLoadingLeavesItUnchanged()
		{
			var data = CreateTestXmlData();
			var xmlMemoryStream = new XmlMemoryStream(data);
			MemoryStream stream = xmlMemoryStream.Save();
			Assert.AreEqual(data.ToXmlString(),
				new XmlMemoryStream(stream).Root.ToXmlString());
		}

		private static XmlData CreateTestXmlData()
		{
			var root = new XmlData("Root");
			var child1 = new XmlData("Child1", root);
			child1.AddAttribute("Attr1", "Value with space");
			child1.AddAttribute("Attr2", "Value2");
			var child2 = new XmlData("Child2", root);
			child2.AddAttribute("Attr3", "Value3");
			child2.AddAttribute("Attr4", "Value4");
			new XmlData("Grandchild", child2);
			return root;
		}
	}
}