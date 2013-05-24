using System;
using DeltaEngine.Content;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests.ModuleMocks;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	public class SoundDeviceTests : TestWithAllFrameworks
	{
		[IntegrationTest, Category("Slow")]
		public void PlayMusicWhileOtherIsPlaying(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var music1 = content.Load<Music>("DefaultMusic");
				var music2 = content.Load<Music>("DefaultMusic");
				if (mockResolver != null)
				{
					music1.Play();
					Assert.False(MultimediaMocks.MusicStopCalled);
					music2.Play();
					Assert.False(MultimediaMocks.MusicStopCalled);
				}
			});
		}

		[IntegrationTest, Category("Slow")]
		public void PlayVideoWhileOtherIsPlaying(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var video1 = content.Load<Video>("DefaultVideo");
				var video2 = content.Load<Video>("DefaultVideo");
				if (mockResolver != null)
				{
					video1.Play();
					Assert.False(MultimediaMocks.VideoStopCalled);
					video2.Play();
					Assert.False(MultimediaMocks.VideoStopCalled);
				}
			});
		}

		[IntegrationTest, Category("Slow")]
		public void RunWithVideoAndMusic(Type resolver)
		{
			Start(resolver, (ContentLoader content, SoundDevice device) =>
			{
				var video = content.Load<Video>("DefaultVideo");
				var music = content.Load<Music>("DefaultMusic");
				if (mockResolver != null)
				{
					video.Play();
					music.Play();
					device.Run();
				}
			});
		}
	}
}
