using System;
using DeltaEngine.Core;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Holds the audio device and automatically disposes all finished playing sound instances.
	/// </summary>
	public abstract class SoundDevice : PriorityRunner, IDisposable
	{
		public virtual void Run()
		{
			if (currentPlayingMusic != null)
				currentPlayingMusic.Run();

			if (currentPlayingVideo != null)
				currentPlayingVideo.Run();
		}

		public abstract void Dispose();

		internal void RegisterCurrentMusic(Music music)
		{
			if (music != null && currentPlayingMusic != null)
				currentPlayingMusic.Stop();

			currentPlayingMusic = music;
		}

		private Music currentPlayingMusic;

		public void RegisterCurrentVideo(Video video)
		{
			if (video != null && currentPlayingVideo != null)
				currentPlayingVideo.Stop();

			currentPlayingVideo = video;
		}

		private Video currentPlayingVideo;
	}
}