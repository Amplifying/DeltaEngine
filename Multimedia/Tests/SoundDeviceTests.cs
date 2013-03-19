using System;
using DeltaEngine.Core;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	public class SoundDeviceTests : TestStarter
	{
		[IntegrationTest]
		public void PlayMusicWhileOtherIsPlaying(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var music1 = content.Load<Music>("DefaultMusic");
				var music2 = content.Load<Music>("DefaultMusic");
				if (testResolver != null)
				{
					music1.Play();
					Assert.False(testResolver.IsMusicStopCalled());
					music2.Play();
					Assert.True(testResolver.IsMusicStopCalled());
				}
			});
		}
	}
}
