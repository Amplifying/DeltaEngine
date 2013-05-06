using System;
using System.Diagnostics;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Platforms.All;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	/// <summary>
	/// Test music playback. Xna music loading won't work from ReSharper, use Program.cs instead.
	/// </summary>
	public class MusicTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void PlayMusic(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var music = content.Load<Music>("DefaultMusic");
				music.Play();
				//music.Dispose();
			});
		}

		[IntegrationTest]
		public void StartAndStopMusic(Type resolver)
		{
			Music music = null;
			Start(resolver, (ContentLoader content) =>
			{
				music = content.Load<Music>("DefaultMusic");
				Assert.Less(4.12f, music.DurationInSeconds);
				Assert.Greater(4.14f, music.DurationInSeconds);
				music.Play();
				Assert.IsTrue(music.IsPlaying());
			}, () =>
			{
				if (Time.Current.Milliseconds >= 1000 || mockResolver != null)
				{
					music.Stop();
					Assert.IsFalse(music.IsPlaying());
					Assert.Less(0.99f, music.PositionInSeconds);
					Assert.Greater(1.01f, music.PositionInSeconds);
				}
			});
		}

		[IntegrationTest]
		public void ShouldThrowIfMusicNotLoadedInDebugModeOrWithDebuggerAttached(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				if (!Debugger.IsAttached || resolver.FullName.Contains("Mock"))
					return;
				//ncrunch: no coverage start
				Assert.Throws<ContentLoader.ContentNotFound>(() => content.Load<Music>("UnavailableMusic"));
			});
		}
	}
}