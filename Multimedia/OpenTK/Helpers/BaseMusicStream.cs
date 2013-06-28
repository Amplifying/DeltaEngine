using System;

namespace DeltaEngine.Multimedia.OpenTK.Helpers
{
	public interface BaseMusicStream : IDisposable
	{
		int Channels { get; }
		int Samplerate { get; }
		float LengthInSeconds { get; }

		int Read(byte[] buffer, int length);
	}
}