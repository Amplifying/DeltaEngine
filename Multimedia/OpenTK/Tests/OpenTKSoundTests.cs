using System;
using DeltaEngine.Core.Content;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.OpenTK.Tests
{
	public class OpenTKSoundTests
	{
		[Test, Category("Slow")]
		public void TestCreation()
		{
			App.Start((OpenTKSound sound) => Assert.NotNull(sound));
		}

		[Test]
		public void TestLoadWithInvalidFilename()
		{
			var sound = new OpenTKSound(null);
			Assert.Throws(typeof(ArgumentException), () => sound.Load(null));
			Assert.Throws(typeof(ArgumentException), () => sound.Load(""));
			Assert.Throws(typeof(ArgumentException), () => sound.Load("?:\\testsound.wav"));
		}

		[Test, Category("Slow")]
		public void TestLoad()
		{
			App.Start((OpenTKSound sound) => sound.Load("../../testsound.wav"));
			
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void PlaySound()
		{
			throw new NotImplementedException("TODO");
			//TODO: App.Start((ContentLoader content) => content.Load<Sound>("testsound").Play());

			App.Start(delegate(OpenTKSound sound)
			{
				sound.Load("../../testsound.wav");
				sound.Play();
			});
		}
	}
}
