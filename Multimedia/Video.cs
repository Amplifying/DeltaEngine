using DeltaEngine.Core;
using DeltaEngine.Rendering;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Provides a way to load and play a video file.
	/// </summary>
	public abstract class Video : ContentData
	{
		protected Video(string filename, Renderer renderer)
			: base(filename)
		{
			this.renderer = renderer;
		}

		protected readonly Renderer renderer;

		public void Play(float volume = 1f)
		{
			surface = PlayNativeVideo(volume);
			renderer.Add(surface);
		}

		private VideoSurface surface;

		protected internal abstract VideoSurface PlayNativeVideo(float volume);

		public void Stop()
		{
			renderer.Remove(surface);
			surface = null;
			StopNativeVideo();
		}

		protected internal abstract void StopNativeVideo();
		public abstract bool IsPlaying();

		protected internal abstract void Run();

		public abstract float DurationInSeconds { get; }
		public abstract float PositionInSeconds { get; }

		public override void Dispose()
		{
			Stop();
		}
	}
}