using System;
using DeltaEngine.Core;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	/// <summary>
	/// Test video playback. Xna video loading won't work from ReSharper, use Program.cs instead.
	/// </summary>
	public class VideoTests : TestStarter
	{
		[VisualTest]
		public void PlayVideo(Type resolver)
		{
			Start(resolver, (Content content) => content.Load<Video>("DefaultVideo").Play());
		}

		[VisualTest]
		public void StartAndStopVideo(Type resolver)
		{
			Video video = null;
			Start(resolver, (Content content, Renderer renderer) =>
			{
				Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
				video = content.Load<Video>("DefaultVideo");
				Assert.AreEqual(3.791f, video.DurationInSeconds);
				video.Play();
			}, (Renderer renderer, Time time) =>
			{
				if (time.Milliseconds >= 1000 || testResolver != null)
				{
					video.Stop();
					Assert.AreEqual(1.0f, video.PositionInSeconds);
					Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
				}
			});
		}
	}
}