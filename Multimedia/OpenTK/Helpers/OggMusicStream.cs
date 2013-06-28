using System.IO;
using DeltaEngine.Core;
using NVorbis;

namespace DeltaEngine.Multimedia.OpenTK.Helpers
{
	internal class OggMusicStream : BaseMusicStream
	{
		public OggMusicStream(Stream stream)
		{
			reader = new VorbisReader(stream, false);
		}

		private VorbisReader reader;

		public void Dispose()
		{
			reader.Dispose();
			reader = null;
		}

		public int Channels
		{
			get { return reader.Channels; }
		}

		public int Samplerate
		{
			get { return reader.SampleRate; }
		}

		public float LengthInSeconds
		{
			get { return (float)reader.TotalTime.TotalSeconds; }
		}

		public int Read(byte[] buffer, int length)
		{
			float[] sampleBuffer = new float[length / 2];
			int count = reader.ReadSamples(sampleBuffer, 0, sampleBuffer.Length);
			int targetIndex = 0;
			for (int i = 0; i < count; i++)
			{
				short sample = FloatToShortRange(sampleBuffer[i]);
				buffer[targetIndex++] = (byte)(sample & 0x00FF);
				buffer[targetIndex++] = (byte)((sample >> 8) & 0x00FF);
			}

			return count * 2;
		}

		private short FloatToShortRange(float value)
		{
			int temp = (int)(short.MaxValue * value);
			return (short)temp.Clamp(short.MinValue, short.MaxValue);
		}
	}
}