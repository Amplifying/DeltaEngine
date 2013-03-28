using DeltaEngine.Core;

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
		public abstract void Stop();
		public abstract bool IsPlaying { get; }
		protected abstract void Run();
		internal void InternalRun()
		{
			Run();
		}
		public abstract float DurationInSeconds { get; }
		public abstract float PositionInSeconds { get; }
	}
}