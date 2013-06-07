using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	public class VectorTextTests : TestWithAllFrameworks
	{
		[VisualTest, ApproveFirstFrameScreenshot]
		public void DrawHi(Type resolver)
		{
			Start(resolver, () => new VectorText("Hi", Point.Half));
		}

		[VisualTest, ApproveFirstFrameScreenshot]
		public void DrawSampleText(Type resolver)
		{
			Start(resolver, () =>
			{
				new VectorText("The Quick Brown Fox...", Point.Half) { Color = Color.Red };
				new VectorText("Jumps Over The Lazy Dog", new Point(0.5f, 0.6f)) { Color = Color.Teal };
			});
		}

		[VisualTest, ApproveFirstFrameScreenshot]
		public void DrawBigText(Type resolver)
		{
			Start(resolver,
				() => new VectorText("Yo yo, whats up", Point.Half) { Size = new Size(0.1f) });
		}

		[Test]
		public void CountNumberOfDrawsWithOneText()
		{
			Start(typeof(MockResolver),
				() => { new VectorText("Yo yo, whats up", Point.Half) { Size = new Size(0.1f) }; });
			Assert.AreEqual(1, mockResolver.rendering.NumberOfTimesDrawn);
		}

		[Test]
		public void CountNumberOfDrawsWithTwoTexts()
		{
			Start(typeof(MockResolver), () =>
			{
				new VectorText("Yo yo, whats up", Point.Half) { Size = new Size(0.1f) };
				new VectorText("Jumps Over The Lazy Dog", new Point(0.5f, 0.6f)) { Color = Color.Teal };
			});
			Assert.AreEqual(2, mockResolver.rendering.NumberOfTimesDrawn);
		}
	}
}