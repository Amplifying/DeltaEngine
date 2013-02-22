using NUnit.Framework;

namespace DeltaEngine.Core.Xml.Tests
{
	public class XmlDataTests
	{
		[Test]
		public void Constructor()
		{
			var root = new XmlData("name");
			Assert.AreEqual("name", root.Name);
			Assert.AreEqual(0, root.Children.Count);
			Assert.AreEqual(0, root.Attributes.Count);
		}

		[Test]
		public void Name()
		{
			var root = new XmlData("name") { Name = "new name" };
			Assert.AreEqual("new name", root.Name);
		}

		[Test]
		public void GetChild()
		{
			var root = new XmlData("root");
			new XmlData("child1", root);
			var child2 = new XmlData("child2", root);
			Assert.AreEqual(child2, root.GetChild("child2"));
		}

		[Test]
		public void GetChildren()
		{
			var root = new XmlData("root");
			var child1 = new XmlData("child", root);
			new XmlData("stepchild", root);
			var child2 = new XmlData("child", root);
			var children = root.GetChildren("child");
			Assert.AreEqual(2, children.Count);
			Assert.IsTrue(children.Contains(child1));
			Assert.IsTrue(children.Contains(child2));
		}

		[Test]
		public void AddAttribute()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute", "value");
			Assert.AreEqual(1, root.Attributes.Count);
			Assert.AreEqual(new XmlAttribute("attribute", "value"), root.Attributes[0]);
		}

		[Test]
		public void RemoveAttributes()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute1", "value1");
			root.AddAttribute("attribute2", "value2");
			root.AddAttribute("attribute1", "value3");
			root.RemoveAttributes("attribute1");

			Assert.AreEqual(1, root.Attributes.Count);
		}

		[Test]
		public void RemoveNonExistentAttributes()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute1", "value1");
			root.AddAttribute("attribute2", "value2");
			root.AddAttribute("attribute1", "value3");
			root.RemoveAttributes("attribute3");
			Assert.AreEqual(3, root.Attributes.Count);
		}

		[Test]
		public void GetValue()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute", "value");
			Assert.AreEqual("value", root.GetValue("attribute"));
		}

		[Test]
		public void NonExistentValueIsNull()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute", "value");
			Assert.AreEqual(null, root.GetValue("attribute2"));
		}

		[Test]
		public void ToXmlStringOneChildWithOneAttribute()
		{
			var trading = new XmlData("Trading");
			var animal = new XmlData("AnimalPrice", trading);
			animal.AddAttribute("ZooDollar", 5);
			Assert.AreEqual(Header + ChildOneAttr, trading.ToXmlString());
		}

		private const string Header = @"<?xml version=""1.0"" encoding=""utf-8""?>
";
		private const string ChildOneAttr = @"<Trading>
	<AnimalPrice ZooDollar=""5"" />
</Trading>";

		[Test]
		public void ToXmlStringRootHasOneAttributeAndAChildWithNoAttributes()
		{
			var root = new XmlData("Root");
			root.AddAttribute("Attribute", "Value");
			new XmlData("Child", root);
			Assert.AreEqual(Header + RootOneAttr + EmptyChild + EndRoot, root.ToXmlString());
		}

		private const string RootOneAttr = @"<Root Attribute=""Value"">
";
		private const string EmptyChild = @"	<Child />
";
		private const string EndRoot = "</Root>";

		[Test]
		public void ToXmlStringOneChildAndOneGrandchildWithOneAttribute()
		{
			var root = new XmlData("Root");
			var child = new XmlData("Child", root);
			var grandchild = new XmlData("Grandchild", child);
			grandchild.AddAttribute("Attribute", "Value");
			Assert.AreEqual(Header + Root + Child + GrandChild + EndChild + EndRoot, root.ToXmlString());
		}

		private const string Root = @"<Root>
";
		private const string Child = @"	<Child>
";
		private const string GrandChild = @"		<Grandchild Attribute=""Value"" />
";
		private const string EndChild = @"	</Child>
";

		[Test]
		public void ToXmlStringTwoChildrenEachWithTwoAttributes()
		{
			var root = CreateTestXmlData();
			Assert.AreEqual(Header + Root + ChildTwoAttr1 + ChildTwoAttr2 + EndRoot, root.ToXmlString());
		}

		private const string ChildTwoAttr1 = @"	<Child1 Attr1=""Value1"" Attr2=""Value2"" />
";
		private const string ChildTwoAttr2 = @"	<Child2 Attr3=""Value3"" Attr4=""Value4"" />
";

		private XmlData CreateTestXmlData()
		{
			var root = new XmlData("Root");
			var child1 = new XmlData("Child1", root);
			child1.AddAttribute("Attr1", "Value1");
			child1.AddAttribute("Attr2", "Value2");
			var child2 = new XmlData("Child2", root);
			child2.AddAttribute("Attr3", "Value3");
			child2.AddAttribute("Attr4", "Value4");

			return root;
		}
	}
}