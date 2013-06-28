using System.IO;
using DeltaEngine.Multimedia;

namespace DeltaEngine.Platforms.Mocks
{
	public class MockVideo : Video
	{
		public MockVideo(string filename, SoundDevice device)
			: base(filename, device) {}

		protected override void LoadData(Stream fileData) {}

		protected override void PlayNativeVideo(float volume)
		{
			VideoStopCalled = false;
			surface = new MockVideoSurface();
		}

		public static bool VideoStopCalled { get; private set; }

		private MockVideoSurface surface;

		protected internal override void Run() {}

		protected override void StopNativeVideo()
		{
			VideoStopCalled = true;
			if (surface != null)
				surface.IsActive = false;

			surface = null;
		}

		public override bool IsPlaying()
		{
			return !VideoStopCalled;
		}

		public override float DurationInSeconds
		{
			get { return 3.33333325f; }
		}

		public override float PositionInSeconds
		{
			get { return 1.0f; }
		}
	}
}