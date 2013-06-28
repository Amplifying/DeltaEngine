using System.IO;
using ToyMp3;

namespace DeltaEngine.Multimedia.GLFW.Helpers
{
	internal class Mp3MusicStream : BaseMusicStream
	{
		public Mp3MusicStream(Stream stream)
		{
			baseStream = new Mp3Stream(stream);
		}

		private Mp3Stream baseStream;

		public void Dispose()
		{
			baseStream = null;
		}

		public int Channels
		{
			get { return baseStream.Channels; }
		}

		public int Samplerate
		{
			get { return baseStream.Samplerate; }
		}

		public float LengthInSeconds
		{
			get { return baseStream.LengthInSeconds; }
		}

		public int Read(byte[] buffer, int length)
		{
			return baseStream.Read(buffer, length);
		}
	}
}