using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	/// <summary>
	/// Test video playback. Xna video loading won't work from ReSharper, use Program.cs instead.
	/// </summary>
	[Ignore]
	public class VideoTests : TestWithMocksOrVisually
	{
		[Test]
		public void PlayVideo()
		{
			ContentLoader.Load<Video>("DefaultVideo").Play();
		}

		[Test]
		public void PlayAndStop()
		{
			var video = ContentLoader.Load<Video>("DefaultVideo");
			video.Stop();
			Assert.IsFalse(video.IsPlaying());
			video.Play();
			Assert.IsTrue(video.IsPlaying());
		}

		[Test]
		public void PlayAndStopWithEntitySystem()
		{
			var video = ContentLoader.Load<Video>("DefaultVideo");
			Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
			video.Stop();
			Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
			video.Play();
			Assert.AreEqual(1, EntitySystem.Current.NumberOfEntities);
			video.Stop();
			Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
		}

		[Test]
		public void CheckDurationAndPosition()
		{
			var video = ContentLoader.Load<Video>("DefaultVideo");
			video.Run();
			Assert.AreEqual(3.791f, video.DurationInSeconds, 0.5f);
			Assert.AreEqual(1.0f, video.PositionInSeconds);
		}

		[Test]
		public void StartAndStopVideo()
		{
			var video = ContentLoader.Load<Video>("DefaultVideo");
			Assert.AreEqual(3.791f, video.DurationInSeconds, 0.5f);
			video.Play();
			RunCode = () =>
			{
				if (Time.Current.Milliseconds >= 1000)
				{
					video.Stop();
					Assert.Less(0.99f, video.PositionInSeconds);
					Assert.Greater(1.04f, video.PositionInSeconds);
				}
			};
		}
	}
}