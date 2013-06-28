using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Entities.Tests
{
	public class BinaryDataExtensionsTests
	{
		[SetUp]
		public void SetUpContentLoader()
		{
			
		}

		[Test]
		public void GetShortName()
		{
			Assert.AreEqual("MockBinaryData", new MockBinaryData("Test").GetShortName());
			CheckConflictingShortNames();
			Assert.Throws<KeyNotFoundException>(() => new DateTime(0).GetShortName());
		}

		public class MockBinaryData
		{
			public MockBinaryData(string text)
				: this()
			{
				Text = text;
			}

			public string Text { get; private set; }

			private MockBinaryData() {}
		}

		private static void CheckConflictingShortNames()
		{
			Assert.AreEqual("Color", new Color().GetShortName());
			Assert.AreEqual("DeltaEngine.Datatypes.Color", Datatypes.Color.Red.GetShortName());
		}

		private class Color {}

		[Test]
		public void Create()
		{
			MemoryStream data = new MockBinaryData("Hallo").SaveToMemoryStream();
			var reconstructed = data.CreateFromMemoryStream() as MockBinaryData;
			Assert.AreEqual("Hallo", reconstructed.Text);
			byte[] bytes = data.ToArray();
			Assert.AreEqual('M', bytes[1]);
		}

		[Test]
		public void SaveAndLoadMockData()
		{
			MemoryStream data = new MockBinaryData("Hallo").SaveToMemoryStream();
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(22, savedBytes.Length);
			Assert.AreEqual(14, savedBytes[0]);
			Assert.IsTrue(savedBytes[15] != 0);
			Assert.AreEqual(5, savedBytes[16]);
			Assert.AreEqual('H', (char)savedBytes[17]);
			var reconstructed = new MemoryStream(savedBytes).CreateFromMemoryStream() as MockBinaryData;
			Assert.AreEqual("Hallo", reconstructed.Text);
		}

		[Test]
		public void ToByteArray()
		{
			var data = new MockBinaryData("Hi");
			byte[] bytes = data.ToByteArrayWithTypeInformation();
			Assert.AreEqual(19, bytes.Length);
			Assert.AreEqual(14, bytes[0]);
			Assert.IsTrue(bytes[15] != 0);
			Assert.AreEqual(2, bytes[16]);
			Assert.AreEqual('H', (char)bytes[17]);
			var reconstructed = bytes.ToBinaryData() as MockBinaryData;
			Assert.AreEqual("Hi", reconstructed.Text);
		}

		[Test]
		public void ToByteArrayWithLengthHeader()
		{
			var data = new MockBinaryData("Hi");
			byte[] bytes = data.ToByteArrayWithLengthHeader();
			Assert.AreEqual(23, bytes.Length);
			var fullNetworkMessage = new MemoryStream(bytes);
			var lengthBytes = new byte[4];
			fullNetworkMessage.Read(lengthBytes, 0, 4);
			Assert.AreEqual(19, lengthBytes[0]);
			var reconstructed = new BinaryReader(fullNetworkMessage).Create() as MockBinaryData;
			Assert.AreEqual("Hi", reconstructed.Text);
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void ModifyingDataCannotBeReloaded()
		{
			MemoryStream data = new MockBinaryData("Hallo").SaveToMemoryStream();
			byte[] bytes = data.ToArray();
			bytes[1] = (byte)'x';
			Assert.Throws<BinaryDataExtensions.UnknownMessageTypeReceived>(
				() => new MemoryStream(bytes).CreateFromMemoryStream());
		}

		[Test, Category("Slow")]
		public void CheckMissingMethodException()
		{
			MemoryStream data = new ClassWithoutDefaultConstructor("Hallo").SaveToMemoryStream();
			Assert.Throws<MissingMethodException>(() => data.CreateFromMemoryStream());
		}

		private class ClassWithoutDefaultConstructor
		{
			public ClassWithoutDefaultConstructor(string text)
			{
				Assert.IsNotNull(text);
			}
		}
	}
}