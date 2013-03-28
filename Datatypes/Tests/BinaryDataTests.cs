using System;
using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	public class BinaryDataTests
	{
		[Test]
		public void GetShortName()
		{
			Assert.AreEqual("MockBinaryData", new MockBinaryData("Test").GetShortName());
			CheckConflictingShortNames();
		}

		private static void CheckConflictingShortNames()
		{
			Assert.AreEqual("Color", new Color().GetShortName());
			Assert.AreEqual("DeltaEngine.Datatypes.Color", Datatypes.Color.Red.GetShortName());
		}

		public class MockBinaryData : BinaryData
		{
			public MockBinaryData(string text) 
				: this()
			{
				Text = text;
			}

			public string Text { get; private set; }

			private MockBinaryData() {}

			public void LoadData(BinaryReader reader)
			{
				Text = reader.ReadString();
			}

			public void SaveData(BinaryWriter writer)
			{
				writer.Write(Text);
			}
		}

		[Test]
		public void Create()
		{
			var data = new MockBinaryData("Hallo").SaveToMemoryStream();
			var reconstructed = data.CreateFromMemoryStream<MockBinaryData>();
			Assert.AreEqual("Hallo", reconstructed.Text);
			byte[] bytes = data.ToArray();
			Assert.AreEqual('M', bytes[1]);
			bytes[1] = (byte)'x';
			Assert.Throws<BinaryDataExtension.UnknownMessageTypeReceived>(
				() => new MemoryStream(bytes).CreateFromMemoryStream<MockBinaryData>());
		}

		[Test]
		public void CheckMissingMethodException()
		{
			var data = new BinaryDataWithoutConstructor("Hallo").SaveToMemoryStream();
			Assert.Throws<MissingMethodException>(() => data.CreateFromMemoryStream<MockBinaryData>());
		}

		public class BinaryDataWithoutConstructor : BinaryData
		{
			public BinaryDataWithoutConstructor(string text)
			{
				Assert.IsNotNull(text);
				LoadData(null);
				SaveData(null);
			}

			public void SaveData(BinaryWriter writer) {}
			public void LoadData(BinaryReader reader) {}
		}


		private class Color : BinaryData
		{
			public Color()
			{
				LoadData(null);
				SaveData(null);
			}

			public void SaveData(BinaryWriter writer) {}
			public void LoadData(BinaryReader reader) {}
		}

		[Test]
		public void SaveAndLoadMockData()
		{
			var data = new MockBinaryData("Hallo").SaveToMemoryStream();
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(21, savedBytes.Length);
			Assert.AreEqual(14, savedBytes[0]);
			Assert.AreEqual(5, savedBytes[15]);
			Assert.AreEqual('H', (char)savedBytes[16]);
			var reconstructed = new MemoryStream(savedBytes).CreateFromMemoryStream<MockBinaryData>();
			Assert.AreEqual("Hallo", reconstructed.Text);
		}

		[Test]
		public void ToByteArray()
		{
			var data = new MockBinaryData("Hi");
			var bytes = data.ToByteArray();
			Assert.AreEqual(18, bytes.Length);
			Assert.AreEqual(14, bytes[0]);
			Assert.AreEqual(2, bytes[15]);
			Assert.AreEqual('H', (char)bytes[16]);
			var reconstructed = bytes.ToBinaryData<MockBinaryData>();
			Assert.AreEqual("Hi", reconstructed.Text);
		}

		[Test]
		public void ToByteArrayWithLengthHeader()
		{
			var data = new MockBinaryData("Hi");
			var bytes = data.ToByteArrayWithLengthHeader();
			Assert.AreEqual(22, bytes.Length);
			var fullNetworkMessage = new MemoryStream(bytes);
			var lengthBytes = new byte[4];
			fullNetworkMessage.Read(lengthBytes, 0, 4);
			Assert.AreEqual(18, lengthBytes[0]);
			var reconstructed = new BinaryReader(fullNetworkMessage).Create<MockBinaryData>();
			Assert.AreEqual("Hi", reconstructed.Text);
		}
	}
}