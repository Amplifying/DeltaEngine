using Microsoft.Xna.Framework.Media;

namespace DeltaEngine.Multimedia.Xna
{
	/// <summary>
	/// Native Xna implementation for music playback.
	/// </summary>
	public class XnaMusic : Music
	{
		public XnaMusic(string filename, XnaSoundDevice device)
			: base(filename, device)
		{
			music = device.Content.Load<Song>(filename);
		}

		private Song music;

		protected override void PlayNativeMusic(float volume)
		{
			positionInSeconds = 0f;
			MediaPlayer.Volume = volume;
			MediaPlayer.Play(music);
		}

		private float positionInSeconds;

		public override void Stop()
		{
			MediaPlayer.Stop();
		}

		public override bool IsPlaying
		{
			get { return MediaPlayer.State != MediaState.Stopped && IsActiveMusic(); }
		}

		private bool IsActiveMusic()
		{
			return MediaPlayer.Queue.Count > 0 && MediaPlayer.Queue.ActiveSong == music;
		}

		protected override void Run()
		{
			if (IsActiveMusic())
				positionInSeconds = (float)MediaPlayer.PlayPosition.TotalSeconds;
		}

		public override void Dispose()
		{
			if (music == null)
				return;

			music.Dispose();
			music = null;
		}

		public override float DurationInSeconds
		{
			get { return (float)music.Duration.TotalSeconds; }
		}

		public override float PositionInSeconds
		{
			get { return positionInSeconds; }
		}
	}
}