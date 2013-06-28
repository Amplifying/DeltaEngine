using System.Diagnostics;
using System.Threading;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	/// <summary>
	/// Test music playback. Xna music loading won't work from ReSharper, use Program.cs instead.
	/// </summary>
	[Ignore]
	public class MusicTests : TestWithMocksOrVisually
	{
		[Test]
		public void PlayMusic()
		{
			ContentLoader.Load<Music>("DefaultMusic").Play();
		}

		[Test]
		public void PlayMusicWith5Fps()
		{
			var music = ContentLoader.Load<Music>("DefaultMusic");
			music.Play();
			RunCode = () => Thread.Sleep(200);
		}

		[Test]
		public void PlayMusicWith10Fps()
		{
			var music = ContentLoader.Load<Music>("DefaultMusic");
			music.Play();
			RunCode = () => Thread.Sleep(100);
		}

		[Test]
		public void PlayMusicWith30Fps()
		{
			var music = ContentLoader.Load<Music>("DefaultMusic");
			music.Play();
			RunCode = () => Thread.Sleep(1000 / 30);
		}

		[Test]
		public void StartAndStopMusic()
		{
			Music music = ContentLoader.Load<Music>("DefaultMusic");
			Assert.Less(4.12f, music.DurationInSeconds);
			Assert.Greater(4.14f, music.DurationInSeconds);
			music.Play();
			Assert.IsTrue(music.IsPlaying());
			RunCode = () =>
			{
				if (Time.Current.Milliseconds >= 1000)
				{
					music.Stop();
					Assert.IsFalse(music.IsPlaying());
					Assert.Less(0.99f, music.PositionInSeconds);
					Assert.Greater(1.01f, music.PositionInSeconds);
				}
			};
		}

		[Test]
		public void ShouldThrowIfMusicNotLoadedInDebugModeOrWithDebuggerAttached()
		{
			if (!Debugger.IsAttached)
				return;
			//ncrunch: no coverage start
			Assert.Throws<ContentLoader.ContentNotFound>(
				() => ContentLoader.Load<Music>("UnavailableMusic"));
		}
	}
}