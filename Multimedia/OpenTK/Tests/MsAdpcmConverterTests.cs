using System;
using System.IO;
using DeltaEngine.Multimedia.OpenTK.Helpers;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.OpenTK.Tests
{
	public class MsAdpcmConverterTests
	{
		[Test]
		public void ConvertToPcm()
		{
			var converter = new MsAdpcmConverter(1, 4, 8);
			byte[] sourceData = GenerateTestData();
			byte[] result = converter.ConvertToPcm(sourceData);
			Assert.AreEqual(16383, BitConverter.ToInt16(result, 0));
			Assert.AreEqual(32767, BitConverter.ToInt16(result, 2));
			Assert.AreEqual(28743, BitConverter.ToInt16(result, 4));
			Assert.AreEqual(29749, BitConverter.ToInt16(result, 6));
		}

		private static byte[] GenerateTestData()
		{
			byte[] sourceData;
			using (var stream = new MemoryStream())
			{
				var writer = new BinaryWriter(stream);
				writer.Write((byte)3);
				writer.Write((short)24);
				writer.Write((short)32767);
				writer.Write((short)16383);
				writer.Write((byte)(3 << 4));
				sourceData = stream.ToArray();
			}

			return sourceData;
		}
	}
}