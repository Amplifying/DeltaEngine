using System;
using System.IO;
using NUnit.Framework;
using OpenTK.Audio.OpenAL;

namespace DeltaEngine.Multimedia.OpenTK.Tests
{
	public class WaveSoundDataTests
	{
		[Test]
		public void TestParsingWithWrongFirstMagics()
		{
			var stream = new MemoryStream();
			var writer = new BinaryWriter(stream);
			var reader = new BinaryReader(stream);
			writer.Write(new[] { 'A', 'I', 'F', 'F' });
			stream.Position = 0;

			Assert.Throws(typeof(NotSupportedException), () => new WaveSoundData(reader));
		}
		
		[Test]
		public void TestParsingWithWrongSecondMagics()
		{
			var stream = new MemoryStream();
			var writer = new BinaryWriter(stream);
			var reader = new BinaryReader(stream);
			writer.Write(new[] { 'R', 'I', 'F', 'F' });
			writer.Write(15);
			writer.Write(new[] { 'W', 'A', ' ', 'E' });
			stream.Position = 0;

			Assert.Throws(typeof(NotSupportedException), () => new WaveSoundData(reader));
		}

		[Test]
		public void TestParsingWithWrongFormat()
		{
			Stream data = GenerateWrongFormatTestData();
			data.Position = 0;
			var reader = new BinaryReader(data);

			Assert.Throws(typeof(NotSupportedException), () => new WaveSoundData(reader));
		}

		private Stream GenerateWrongFormatTestData()
		{
			var stream = new MemoryStream();
			var writer = new BinaryWriter(stream);
			writer.Write(new[] { 'R', 'I', 'F', 'F' });
			writer.Write(48);
			writer.Write(new[] { 'W', 'A', 'V', 'E' });

			writer.Write(new[] { 'f', 'm', 't', ' ' });
			writer.Write(16);
			writer.Write((short)54);
			writer.Write((short)1);
			writer.Write(44100);
			writer.Write(0);
			writer.Write((short)8);
			writer.Write((short)8);

			return stream;
		}

		[Test]
		public void TestParsing()
		{
			BinaryWriter writer = GenerateDefaultWaveData();
			writer.BaseStream.Position = 0;
			var reader = new BinaryReader(writer.BaseStream);

			var soundData = new WaveSoundData(reader);
			Assert.AreEqual(44100, soundData.SampleRate);
			Assert.AreEqual(ALFormat.Stereo8, soundData.Format);
			Assert.AreEqual(16, soundData.BufferData.Length);
			for (int index = 0; index < 16; index += 2)
			{
				Assert.AreEqual(34, soundData.BufferData[index]);
				Assert.AreEqual(14, soundData.BufferData[index + 1]);
			}
		}

		[Test]
		public void SkipUnknownChunk()
		{
			BinaryWriter writer = GenerateDefaultWaveData();
			writer.Write(new[] { 't', 'e', 's', 't' });
			writer.Write(128);
			writer.Write(new byte[128]);
			writer.BaseStream.Position = 0;
			var reader = new BinaryReader(writer.BaseStream);

			var soundData = new WaveSoundData(reader);
			Assert.AreEqual(ALFormat.Stereo8, soundData.Format);
			Assert.AreEqual(16, soundData.BufferData.Length);
		}

		private BinaryWriter GenerateDefaultWaveData()
		{
			var stream = new MemoryStream();
			var writer = new BinaryWriter(stream);
			writer.Write(new[] { 'R', 'I', 'F', 'F' });
			writer.Write(48);
			writer.Write(new[] { 'W', 'A', 'V', 'E' });
			WriteDefaultWaveHeaderChunk(writer);
			writer.Write(new[] { 'd', 'a', 't', 'a' });
			writer.Write(16);
			for (int index = 0; index < 16; index += 2)
			{
				writer.Write((byte)34);
				writer.Write((byte)14);
			}

			return writer;
		}

		private void WriteDefaultWaveHeaderChunk(BinaryWriter writer)
		{
			writer.Write(new[] { 'f', 'm', 't', ' ' });
			writer.Write(16);
			writer.Write((short)WaveFormat.Pcm);
			writer.Write((short)2);
			writer.Write(44100);
			writer.Write(0);
			writer.Write((short)0);
			writer.Write((short)8);
		}

		[Test]
		public void TestMsadpcmParsing()
		{
			BinaryReader reader = GenerateMsadpcmTestData();
			var soundData = new WaveSoundData(reader);

			Assert.AreEqual(44100, soundData.SampleRate);
			Assert.AreEqual(ALFormat.Mono8, soundData.Format);
			Assert.AreEqual(8, soundData.BufferData.Length);
			Assert.AreEqual(16383, BitConverter.ToInt16(soundData.BufferData, 0));
			Assert.AreEqual(32767, BitConverter.ToInt16(soundData.BufferData, 2));
			Assert.AreEqual(28743, BitConverter.ToInt16(soundData.BufferData, 4));
			Assert.AreEqual(29749, BitConverter.ToInt16(soundData.BufferData, 6));
		}
		
		private BinaryReader GenerateMsadpcmTestData()
		{
			var stream = new MemoryStream();
			var writer = new BinaryWriter(stream);
			writer.Write(new[] { 'R', 'I', 'F', 'F' });
			writer.Write(48);
			writer.Write(new[] { 'W', 'A', 'V', 'E' });
			WriteMsadpcmHeaderChunk(writer);
			writer.Write(new[] { 'd', 'a', 't', 'a' });
			writer.Write(8);
			writer.Write((byte)3);
			writer.Write((short)24);
			writer.Write((short)32767);
			writer.Write((short)16383);
			writer.Write((byte)(3 << 4));

			writer.BaseStream.Position = 0;
			return new BinaryReader(writer.BaseStream);
		}

		private void WriteMsadpcmHeaderChunk(BinaryWriter writer)
		{
			writer.Write(new[] { 'f', 'm', 't', ' ' });
			writer.Write(24);
			writer.Write((short)WaveFormat.MsAdpcm);
			writer.Write((short)1);
			writer.Write(44100);
			writer.Write(0);
			writer.Write((short)8);
			writer.Write((short)8);
			writer.Write((short)4);
			writer.Write(0);
			writer.Write((short)WaveFormat.MsAdpcm);
		}

		[Test]
		public void TestIeeeParsing()
		{
			BinaryReader reader = GenerateIeeeTestData();
			var soundData = new WaveSoundData(reader);

			Assert.AreEqual(44100, soundData.SampleRate);
			Assert.AreEqual(ALFormat.Stereo16, soundData.Format);
			Assert.AreEqual(8, soundData.BufferData.Length);
			Assert.AreEqual(16383, BitConverter.ToInt16(soundData.BufferData, 0));
			Assert.AreEqual(32767, BitConverter.ToInt16(soundData.BufferData, 2));
			Assert.AreEqual(3276, BitConverter.ToInt16(soundData.BufferData, 4));
			Assert.AreEqual(0, BitConverter.ToInt16(soundData.BufferData, 6));
		}

		private BinaryReader GenerateIeeeTestData()
		{
			var stream = new MemoryStream();
			var writer = new BinaryWriter(stream);
			writer.Write(new[] { 'R', 'I', 'F', 'F' });
			writer.Write(48);
			writer.Write(new[] { 'W', 'A', 'V', 'E' });
			WriteIeeeHeaderChunk(writer);
			writer.Write(new[] { 'd', 'a', 't', 'a' });
			writer.Write(16);
			writer.Write(0.5f);
			writer.Write(1.0f);
			writer.Write(0.1f);
			writer.Write(0.0f);

			writer.BaseStream.Position = 0;
			return new BinaryReader(writer.BaseStream);
		}

		private void WriteIeeeHeaderChunk(BinaryWriter writer)
		{
			writer.Write(new[] { 'f', 'm', 't', ' ' });
			writer.Write(16);
			writer.Write((short)WaveFormat.IeeeFloat);
			writer.Write((short)2);
			writer.Write(44100);
			writer.Write(0);
			writer.Write((short)0);
			writer.Write((short)32);
		}
	}
}
