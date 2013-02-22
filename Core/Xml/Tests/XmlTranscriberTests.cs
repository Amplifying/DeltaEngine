using NUnit.Framework;

namespace DeltaEngine.Core.Xml.Tests
{
	public class XmlTranscriberTests
	{
		[Test]
		public void SanityChecks()
		{
			Assert.Throws<XmlTranscriber.InvalidXmlStringException>(() => new XmlTranscriber(null));
			Assert.Throws<XmlTranscriber.InvalidXmlStringException>(() => new XmlTranscriber(""));
			Assert.Throws<XmlTranscriber.InvalidXmlStringException>(() => new XmlTranscriber("<?xml>"));
		}

		[Test]
		public void PaddingIsNotNeededForValidXml()
		{
			XmlData xmlData = new XmlTranscriber(Header + Unpadded).Result;
			Assert.AreEqual("Animals", xmlData.Name);
		}

		private const string Header = @"<?xml version=""1.0"" encoding=""utf-8""?>
";
		private const string Unpadded = "<Animals><Tiger Size=\"1.5\" /></Animals>";

		[Test]
		public void HeaderIsNotNeededForValidXml()
		{
			XmlData xmlData = new XmlTranscriber(Unpadded).Result;
			Assert.AreEqual("Animals", xmlData.Name);
		}

		[Test]
		public void OneChildWithOneAttributeDetail()
		{
			XmlData xmlData = new XmlTranscriber(Root + Child + EndRoot).Result;
			Assert.AreEqual("Trading", xmlData.Name);
			Assert.AreEqual(1, xmlData.Children.Count);
			Assert.AreEqual("AnimalPrice", xmlData.Children[0].Name);
			Assert.AreEqual(1, xmlData.Children[0].Attributes.Count);
			Assert.AreEqual("ZooDollar", xmlData.Children[0].Attributes[0].Name);
			Assert.AreEqual("5", xmlData.Children[0].Attributes[0].Value);
		}

		private const string Root = @"<Trading>
";
		private const string Child = @"	<AnimalPrice ZooDollar=""5"" />
";
		private const string EndRoot = @"</Trading>";

		[Test]
		public void OneChildWithOneAttributeIsUnchanged()
		{
			XmlData xmlData = new XmlTranscriber(Root + Child + EndRoot).Result;
			Assert.AreEqual(Header + Root + Child + EndRoot, xmlData.ToXmlString());
		}

		[Test]
		public void OneAttributeAndAChildWithNoAttributesIsUnchanged()
		{
			XmlData xmlData = new XmlTranscriber(Root + EmptyChild + EndRoot).Result;
			Assert.AreEqual(Header + Root + EmptyChild + EndRoot, xmlData.ToXmlString());
		}

		private const string EmptyChild = @"	<Child />
";

		[Test]
		public void OneChildAndOneGrandchildWithOneAttributeIsUnchanged()
		{
			const string Xml = Header + Root + BeginChild + Grandchild + EndChild + EndRoot;
			XmlData xmlData = new XmlTranscriber(Xml).Result;
			Assert.AreEqual(Xml, xmlData.ToXmlString());
		}

		private const string BeginChild = @"	<Child>
";
		private const string Grandchild = @"		<Grandchild Attribute=""Value"" />
";
		private const string EndChild = @"	</Child>
";

		[Test]
		public void TwoChildrenEachWithTwoAttributesIsUnchanged()
		{
			const string Xml = Header + Root + ChildTwoAttr1 + ChildTwoAttr2 + EndRoot;
			XmlData xmlData = new XmlTranscriber(Xml).Result;
			Assert.AreEqual(Xml, xmlData.ToXmlString());
		}

		private const string ChildTwoAttr1 = @"	<Child1 Attr1=""Value1"" Attr2=""Value2"" />
";
		private const string ChildTwoAttr2 = @"	<Child2 Attr3=""Value3"" Attr4=""Value4"" />
";
	}
}