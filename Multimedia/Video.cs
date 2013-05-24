using DeltaEngine.Content;
using System;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Provides a way to load and play a video file.
	/// </summary>
	public abstract class Video : ContentData
	{
		protected Video(string filename, SoundDevice device)
			: base(filename)
		{
			this.device = device;
		}

		protected readonly SoundDevice device;

		public void Play(float volume = 1.0f)
		{
			device.RegisterCurrentVideo(this);
			PlayNativeVideo(volume);
		}

		protected abstract void PlayNativeVideo(float volume);
		public void Stop()
		{
			device.RegisterCurrentVideo(null);
			StopNativeVideo();
		}
		protected abstract void StopNativeVideo();
		public abstract bool IsPlaying();
		protected internal abstract void Run();
		public abstract float DurationInSeconds { get; }
		public abstract float PositionInSeconds { get; }
		protected override void DisposeData()
		{
			Stop();
		}

		//ncrunch: no coverage start
		public class VideoNotFoundOrAccessible : Exception
		{
			public VideoNotFoundOrAccessible(string videoName, Exception innerException)
				: base(videoName, innerException) { }
		}
	}
}