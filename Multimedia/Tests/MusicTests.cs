using System;
using DeltaEngine.Core;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	/// <summary>
	/// Test music playback. Xna music loading won't work from ReSharper, use Program.cs instead.
	/// </summary>
	public class MusicTests : TestStarter
	{
		[IntegrationTest]
		public void PlayMusic(Type resolver)
		{
			Start(resolver, (Content content) => content.Load<Music>("DefaultMusic").Play());
		}

		[IntegrationTest]
		public void StartAndStopMusic(Type resolver)
		{
			Music music = null;
			Start(resolver, (Content content) =>
			{
				music = content.Load<Music>("DefaultMusic");
				Assert.Less(4.12f, music.DurationInSeconds);
				Assert.Greater(4.14f, music.DurationInSeconds);
				music.Play();
			}, (Time time) =>
			{
				if (time.Milliseconds >= 1000 || testResolver != null)
				{
					music.Stop();
					Assert.Less(0.99f, music.PositionInSeconds);
					Assert.Greater(1.01f, music.PositionInSeconds);
				}
			});
		}
	}
}