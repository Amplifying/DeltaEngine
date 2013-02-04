using NUnit.Framework;

namespace DeltaEngine.Multimedia.OpenTK.Tests
{
	public class OggMusicDataTests
	{
		[Test, Category("Slow")]
		public void StreamAndUpdate()
		{
			var musicData = new OggMusicData("../../testmusic.ogg");
			musicData.Stream(0);
			musicData.Update();
		}

		[Test, Category("Slow")]
		public void UpdateWithInvalidHandle()
		{
			var musicData = new OggMusicData("../../testmusic.ogg") { sourceHandle = -1 };
			musicData.EnqueueBuffersBeforePlay();
			musicData.Update();
		}

		[Test, Category("Slow")]
		public void StopAndRewind()
		{
			var musicData = new OggMusicData("../../testmusic.ogg");
			musicData.Stop();
			musicData.Rewind();
		}

		[Test, Category("Slow")]
		public void DisposeWithInvalidHandle()
		{
			var musicData = new OggMusicData("../../testmusic.ogg") { sourceHandle = -1 };
			musicData.Dispose();
		}
	}
}
