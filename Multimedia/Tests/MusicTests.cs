using System;
using System.Threading;
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
		[VisualTest]
		public void PlayMusic(Type resolver)
		{
			Start(resolver, (Content content) => content.Load<Music>("DefaultMusic").Play());
		}

		[VisualTest]
		public void StartAndStopMusic(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var music = content.Load<Music>("DefaultMusic");
				Assert.AreEqual(35.85f, music.DurationInSeconds);
				music.Play();
				if (resolver != typeof(TestResolver))
					Thread.Sleep(1000);
				music.Stop();
				Assert.AreEqual(1.0f, music.PositionInSeconds);
			});
		}
	}
}