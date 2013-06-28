using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	public class VectorTextTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void DrawHi()
		{
			new VectorText("Hi", Point.Half);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawSampleText()
		{
			new VectorText("The Quick Brown Fox...", Point.Half) { Color = Color.Red };
			new VectorText("Jumps Over The Lazy Dog", new Point(0.5f, 0.6f)) { Color = Color.Teal };
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawBigText()
		{
			new VectorText("Yo yo, whats up", Point.Half) { Size = new Size(0.1f) };
		}

		[Test]
		public void DrawingTwoVectorTextsWithTheSameRenderLayerOnlyIssuesOneDrawCall()
		{
			new VectorText("Yo yo, whats up", Point.Half) { Size = new Size(0.1f) };
			new VectorText("Jumps Over The Lazy Dog", new Point(0.5f, 0.6f)) { Color = Color.Teal };
			RunCode = () => Assert.AreEqual(1, Resolve<Drawing>().NumberOfTimesDrawn);
			Window.CloseAfterFrame();
		}

		[Test]
		public void DrawingTwoVectorTextsWithDifferentRenderLayersIssuesTwoDrawCalls()
		{
			new VectorText("Yo yo, whats up", Point.Half) { Size = new Size(0.1f), RenderLayer = 1 };
			new VectorText("Jumps Over The Lazy Dog", Point.One) { Color = Color.Teal, RenderLayer = 2 };
			RunCode = () => Assert.AreEqual(2, Resolve<Drawing>().NumberOfTimesDrawn);
			Window.CloseAfterFrame();
		}
	}
}