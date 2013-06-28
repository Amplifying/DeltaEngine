using System.IO;
using DeltaEngine.Multimedia;

namespace DeltaEngine.Platforms.Mocks
{
	public class MockMusic : Music
	{
		public MockMusic(string filename, SoundDevice device, Settings settings)
			: base(filename, device, settings) {}

		protected override void LoadData(Stream fileData) {}

		protected override void PlayNativeMusic(float volume)
		{
			MusicStopCalled = false;
		}

		public static bool MusicStopCalled { get; private set; }

		protected internal override void Run() {}

		protected override void StopNativeMusic()
		{
			MusicStopCalled = true;
		}

		public override bool IsPlaying()
		{
			return !MusicStopCalled;
		}

		public override float DurationInSeconds
		{
			get { return 4.13f; }
		}

		public override float PositionInSeconds
		{
			get { return 1.0f; }
		}
	}
}