using System;
using DeltaEngine.Core;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Holds the audio device and automatically disposes all finished playing sound instances.
	/// </summary>
	public abstract class SoundDevice : Runner, IDisposable
	{
		public virtual void Run()
		{
			if (currentPlayingMusic != null)
				currentPlayingMusic.InternalRun();
		}

		public abstract void Dispose();

		internal void RegisterCurrentMusic(Music music)
		{
			if (currentPlayingMusic != null)
				currentPlayingMusic.Stop();

			currentPlayingMusic = music;
		}

		private Music currentPlayingMusic;
	}
}