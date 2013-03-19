using NUnit.Framework;

namespace DeltaEngine.Core.Xml.Tests
{
	public class XmlSnippetTests
	{
		[Test]
		public void ToAndFromXmlSnippetLeavesTextUnchanged()
		{
			var snippet = CreateTestXmlData().ToXmlString();
			var xmlSnippet = new XmlSnippet(snippet);
			Assert.AreEqual(snippet, xmlSnippet.Root.ToXmlString());
		}

		private static XmlData CreateTestXmlData()
		{
			var root = new XmlData("Root");
			var child1 = new XmlData("Child1", root);
			child1.AddAttribute("Attr1", "Value with space");
			child1.AddAttribute("Attr2", "Value2");
			child1.Value = "Value";
			var child2 = new XmlData("Child2", root);
			child2.AddAttribute("Attr3", "Value3");
			child2.AddAttribute("Attr4", "Value4");
			new XmlData("Grandchild", child2);
			return root;
		}

		[Test]
		public void IgnoresLeadingJunk()
		{
			var snippet = CreateTestXmlData().ToXmlString();
			var xmlSnippet = new XmlSnippet("blahblah" + snippet);
			Assert.AreEqual(snippet, xmlSnippet.Root.ToXmlString());
		}
	}
}