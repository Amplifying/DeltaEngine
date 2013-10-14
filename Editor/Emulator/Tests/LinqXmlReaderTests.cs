using System;
using System.Xml.Linq;
using NUnit.Framework;

namespace DeltaEngine.Editor.Emulator.Tests
{
	public class LinqXmlReaderTests
	{
		[Test]
		public void ReadStringValue()
		{
			var xmlContents = new XElement("xmlContents");
			xmlContents.Add(new XElement("StringValue", "Test"));
			var value = xmlContents.ReadStringValue("StringValue");
			Assert.AreEqual("Test", value);
		}

		[Test]
		public void ReadPointValue()
		{
			var xmlContents = new XElement("xmlContents");
			xmlContents.Add(new XElement("PointValue", "100,200"));
			var value = xmlContents.ReadPointValue("PointValue");
			Assert.AreEqual(100, value.X);
			Assert.AreEqual(200, value.Y);
		}

		[Test]
		public void ReadNonExistingPointValue()
		{
			var xmlContents = new XElement("xmlContents");
			var value = xmlContents.ReadPointValue("PointValue");
			Assert.AreEqual(0, value.X);
			Assert.AreEqual(0, value.Y);
		}

		[Test]
		public void ReadInvalidPointValueCrashes()
		{
			var xmlContents = new XElement("xmlContents");
			xmlContents.Add(new XElement("PointValue", "a,b"));
			Assert.Throws<FormatException>(() => xmlContents.ReadPointValue("PointValue"));
		}

		[Test]
		public void ReadSizeValue()
		{
			var xmlContents = new XElement("xmlContents");
			xmlContents.Add(new XElement("SizeValue", "100,200"));
			var value = xmlContents.ReadSizeValue("SizeValue");
			Assert.AreEqual(100, value.Width);
			Assert.AreEqual(200, value.Height);
		}

		[Test]
		public void ReadNonExistingSizeValue()
		{
			var xmlContents = new XElement("xmlContents");
			var value = xmlContents.ReadSizeValue("SizeValue");
			Assert.AreEqual(0, value.Width);
			Assert.AreEqual(0, value.Height);
		}

		[Test]
		public void ReadIntValue()
		{
			var xmlContents = new XElement("xmlContents");
			xmlContents.Add(new XElement("IntValue", "100"));
			var value = xmlContents.ReadIntValue("IntValue");
			Assert.AreEqual(100, value);
		}

		[Test]
		public void ReadBoolValue()
		{
			var xmlContents = new XElement("xmlContents");
			xmlContents.Add(new XElement("TrueValue", "1"));
			xmlContents.Add(new XElement("FalseValue", "0"));
			var trueValue = xmlContents.ReadBoolValue("TrueValue");
			var falseValue = xmlContents.ReadBoolValue("FalseValue");
			Assert.AreEqual(true, trueValue);
			Assert.AreEqual(false, falseValue);
		}
	}
}