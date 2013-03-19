using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	internal class UnknownBinaryTests
	{
		[Test]
		public void LoadError()
		{
			UnknownBinaryData data = new UnknownBinaryData();
			byte[] savedData = new byte[] { 5, (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o' };
			using (var dataStream = new MemoryStream(savedData))
			{
				var reader = new BinaryReader(dataStream);
				data.Load(reader);
			}
			Assert.AreEqual("Hello", data.Error);
		}

		[Test]
		public void SaveError()
		{
			UnknownBinaryData data = new UnknownBinaryData("Hello");
			BinaryWriter writer = new BinaryWriter(Stream.Null);
			data.Save(writer);
			Assert.AreEqual("Hello", data.Error);
		}
	}
}