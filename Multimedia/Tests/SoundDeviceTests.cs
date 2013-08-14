using DeltaEngine.Content;
using DeltaEngine.Multimedia.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	public class SoundDeviceTests : TestWithMocksOrVisually
	{
		[Test]
		public void PlayMusicWhileOtherIsPlaying()
		{
			var music1 = ContentLoader.Load<Music>("DefaultMusic");
			var music2 = ContentLoader.Load<Music>("DefaultMusic");
			music1.Play();
			Assert.False(MockMusic.MusicStopCalled);
			music2.Play();
			Assert.False(MockMusic.MusicStopCalled);
		}

		[Test]
		public void PlayVideoWhileOtherIsPlaying()
		{
			var video1 = ContentLoader.Load<Video>("DefaultVideo");
			var video2 = ContentLoader.Load<Video>("DefaultVideo");
			video1.Play();
			Assert.False(MockVideo.VideoStopCalled);
			video2.Play();
			Assert.False(MockVideo.VideoStopCalled);
		}

		[Test]
		public void RunWithVideoAndMusic()
		{
			var video = ContentLoader.Load<Video>("DefaultVideo");
			var music = ContentLoader.Load<Music>("DefaultMusic");
			video.Play();
			music.Play();
		}

		[Test]
		public void PlayMusicAndVideo()
		{
			MockSoundDevice device = new MockSoundDevice();
			var video1 = ContentLoader.Load<Video>("DefaultVideo");
			var music1 = ContentLoader.Load<Music>("DefaultMusic");
			Assert.False(MockVideo.VideoStopCalled);
			Assert.False(MockMusic.MusicStopCalled);
			device.RegisterCurrentVideo(video1);
			device.RegisterCurrentMusic(music1);
			Assert.IsTrue(device.IsActive);
			Assert.IsTrue(device.IsInitialized);
			device.RapidUpdate(0.3f);
			device.Dispose();
		}
	}
}