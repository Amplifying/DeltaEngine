using DeltaEngine.Content;
using System;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Provides a way to load and play a music file.
	/// </summary>
	public abstract class Music : ContentData
	{
		protected Music(string filename, SoundDevice device)
			: base(filename)
		{
			this.device = device;
		}

		protected readonly SoundDevice device;

		public void Play(float volume = 1.0f)
		{
			device.RegisterCurrentMusic(this);
			PlayNativeMusic(volume);
		}

		protected abstract void PlayNativeMusic(float volume);
		public void Stop()
		{
			device.RegisterCurrentMusic(null);
			StopNativeMusic();
		}
		protected abstract void StopNativeMusic();
		public abstract bool IsPlaying();
		protected internal abstract void Run();
		public abstract float DurationInSeconds { get; }
		public abstract float PositionInSeconds { get; }
		protected override void DisposeData()
		{
			Stop();
		}

		//ncrunch: no coverage start
		public class MusicNotFoundOrAccessible : Exception
		{
			public MusicNotFoundOrAccessible(string musicName, Exception innerException)
				: base(musicName, innerException) { }
		}
	}
}