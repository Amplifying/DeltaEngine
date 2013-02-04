using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.OpenTK.Tests
{
	public class IeeeFloatConverterTests
	{
		[Test]
		public void ConvertToPcm64Bit()
		{
			byte[] sourceData = GenerateTestData(new[] { 0.5, 1, 0.1, 0 });
			var converter = new IeeeFloatConverter(64);
			byte[] result = converter.ConvertToPcm(sourceData);
			Assert.AreEqual(16383, BitConverter.ToInt16(result, 0));
			Assert.AreEqual(32767, BitConverter.ToInt16(result, 2));
			Assert.AreEqual(3276, BitConverter.ToInt16(result, 4));
			Assert.AreEqual(0, BitConverter.ToInt16(result, 6));
		}

		[Test]
		public void ConvertToPcm32Bit()
		{
			byte[] sourceData = GenerateTestData(new[] { 0.5f, 1f, 0.1f, 0f });
			var converter = new IeeeFloatConverter(32);
			byte[] result = converter.ConvertToPcm(sourceData);
			Assert.AreEqual(16383, BitConverter.ToInt16(result, 0));
			Assert.AreEqual(32767, BitConverter.ToInt16(result, 2));
			Assert.AreEqual(3276, BitConverter.ToInt16(result, 4));
			Assert.AreEqual(0, BitConverter.ToInt16(result, 6));
		}

		private byte[] GenerateTestData<T>(IEnumerable<T> data)
		{
			byte[] sourceData;
			using (var stream = new MemoryStream())
			{
				var writer = new BinaryWriter(stream);
				foreach (T value in data)
					if (typeof(T) == typeof(float))
						writer.Write(Convert.ToSingle(value));
					else if (typeof(T) == typeof(double))
						writer.Write(Convert.ToDouble(value));

				sourceData = stream.ToArray();
			}

			return sourceData;
		}
	}
}
