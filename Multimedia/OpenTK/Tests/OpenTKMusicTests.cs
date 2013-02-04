using System;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.OpenTK.Tests
{
	public class OpenTKMusicTests
	{
		[Test, Category("Slow")]
		public void Creation()
		{
			App.Start((OpenTKMusic music) => Assert.NotNull(music));
		}

		[Test]
		public void LoadWithInvalidFilename()
		{
			var music = new OpenTKMusic(null);
			Assert.Throws(typeof(ArgumentException), () => music.Load(null));
			Assert.Throws(typeof(ArgumentException), () => music.Load(""));
			Assert.Throws(typeof(ArgumentException), () => music.Load("?:\\testsound.wav"));
		}

		[Test, Category("Slow")]
		public void LoadWithTestFile()
		{
			var music = new OpenTKMusic(null);
			music.Load("../../testmusic.ogg");
			Assert.False(music.IsPlaying);
			Assert.AreEqual(44100, music.MusicData.SampleRate);
			Assert.AreEqual(2, music.MusicData.Channels);
			music.Update();
			music.Stop();
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void Play()
		{
			App.Start(delegate(OpenTKMusic music)
			{
				music.Load("../../testmusic.ogg");
				music.Play();
			});
		}
	}
}
