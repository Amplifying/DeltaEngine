using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	public class BinaryDataTests
	{
		[Test]
		public void SaveMockData()
		{
			using (var dataStream = new MemoryStream())
			{
				var writer = new BinaryWriter(dataStream);
				var data = new MockBinaryData("Hallo");
				data.Save(writer);
				byte[] savedBytes = dataStream.ToArray();
				Assert.AreEqual(6, savedBytes.Length);
				Assert.AreEqual(5, savedBytes[0]);
				Assert.AreEqual('H', (char)savedBytes[1]);
			}
		}

		[Test]
		public void LoadMockData()
		{
			byte[] savedData = new byte[] { 5, (byte)'H', (byte)'a', (byte)'l', (byte)'l', (byte)'o' };
			using (var dataStream = new MemoryStream(savedData))
			{
				var reader = new BinaryReader(dataStream);
				var data = new MockBinaryData();
				data.Load(reader);
				Assert.AreEqual("Hallo", data.Text);
			}
		}
	}
}
