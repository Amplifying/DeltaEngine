using System;
using System.Threading;
using DeltaEngine.Core;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Holds the audio device and automatically disposes all finished playing sound instances.
	/// </summary>
	public abstract class SoundDevice : PriorityRunner, IDisposable
	{
		protected SoundDevice()
		{

			runThread = ThreadExtensions.Start(ThreadRun);
		}

		private readonly Thread runThread;

		private void ThreadRun()
		{
			isRunning = true;
			while (isRunning)
			{
				Thread.Sleep(1);
				if (currentPlayingMusic != null)
					currentPlayingMusic.Run();
			}
		}

		private bool isRunning;

		public virtual void Run()
		{
			if (currentPlayingVideo != null)
				currentPlayingVideo.Run();
		}

		public virtual void Dispose()
		{
			isRunning = false;
			runThread.Abort();
		}

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