using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Networking.Sockets.Tests
{
	internal class UnknownBinaryTests
	{
		[Test]
		public void LoadError()
		{
			var data = new UnknownBinaryData();
			var savedData = new byte[] { 5, (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o' };
			using (var dataStream = new MemoryStream(savedData))
			{
				var reader = new BinaryReader(dataStream);
				data.LoadData(reader);
			}
			Assert.AreEqual("Hello", data.Error);
		}

		[Test]
		public void SaveError()
		{
			var data = new UnknownBinaryData("Hello");
			var writer = new BinaryWriter(Stream.Null);
			data.SaveData(writer);
			Assert.AreEqual("Hello", data.Error);
		}
	}
}