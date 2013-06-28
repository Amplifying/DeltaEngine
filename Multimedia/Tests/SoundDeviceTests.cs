using DeltaEngine.Content;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	public class SoundDeviceTests : TestWithMocksOrVisually
	{
		[Test, Category("Slow")]
		public void PlayMusicWhileOtherIsPlaying()
		{
			var music1 = ContentLoader.Load<Music>("DefaultMusic");
			var music2 = ContentLoader.Load<Music>("DefaultMusic");
			music1.Play();
			Assert.False(MockMusic.MusicStopCalled);
			music2.Play();
			Assert.False(MockMusic.MusicStopCalled);
		}

		[Test, Category("Slow"), Ignore]
		public void PlayVideoWhileOtherIsPlaying()
		{
			var video1 = ContentLoader.Load<Video>("DefaultVideo");
			var video2 = ContentLoader.Load<Video>("DefaultVideo");
			video1.Play();
			Assert.False(MockVideo.VideoStopCalled);
			video2.Play();
			Assert.False(MockVideo.VideoStopCalled);
		}

		[Test, Category("Slow"), Ignore]
		public void RunWithVideoAndMusic()
		{
			var video = ContentLoader.Load<Video>("DefaultVideo");
			var music = ContentLoader.Load<Music>("DefaultMusic");
			video.Play();
			music.Play();
		}
	}
}