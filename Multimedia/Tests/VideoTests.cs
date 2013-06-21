using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	/// <summary>
	/// Test video playback. Xna video loading won't work from ReSharper, use Program.cs instead.
	/// </summary>
	public class VideoTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void PlayVideo(Type resolver)
		{
			Start(resolver, (ContentLoader content) => content.Load<Video>("DefaultVideo").Play());
		}

		[IntegrationTest]
		public void PlayAndStop(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var video = content.Load<Video>("DefaultVideo");
				video.Stop();
				Assert.IsFalse(video.IsPlaying());
				video.Play();
				Assert.IsTrue(video.IsPlaying());
			});
		}

		[IntegrationTest]
		public void PlayAndStopWithEntitySystem(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var video = content.Load<Video>("DefaultVideo");
				Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
				video.Stop();
				Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
				video.Play();
				Assert.AreEqual(1, EntitySystem.Current.NumberOfEntities);
				video.Stop();
				Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
			});
		}

		[IntegrationTest]
		public void CheckDurationAndPosition(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var video = content.Load<Video>("DefaultVideo");
				video.Run();
				Assert.AreEqual(3.791f, video.DurationInSeconds, 0.5f);
				Assert.AreEqual(1.0f, video.PositionInSeconds);
			});
		}

		[VisualTest]
		public void StartAndStopVideo(Type resolver)
		{
			Video video = null;
			Start(resolver, (ContentLoader content) =>
			{
				video = content.Load<Video>("DefaultVideo");
				Assert.AreEqual(3.791f, video.DurationInSeconds, 0.5f);
				video.Play();
			}, () =>
			{
				if (Time.Current.Milliseconds >= 1000 || mockResolver != null)
				{
					video.Stop();
					Assert.Less(0.99f, video.PositionInSeconds);
					Assert.Greater(1.04f, video.PositionInSeconds);
				}
			});
		}
	}
}