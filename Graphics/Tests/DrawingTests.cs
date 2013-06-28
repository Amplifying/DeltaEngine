using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class DrawingTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void DrawRedLine()
		{
			var vertices = CreateLine(Color.Red);
			RunCode = () => Resolve<Drawing>().DrawVertices(VerticesMode.Lines, vertices);
		}

		private VertexPositionColor[] CreateLine(Color color)
		{
			var vertices = new VertexPositionColor[2];
			vertices[0] = new VertexPositionColor(Point.Zero, color);
			vertices[1] = new VertexPositionColor(Window.ViewportPixelSize, color);
			Window.ViewportSizeChanged += s => vertices[1] = new VertexPositionColor(s, Color.Green);
			return vertices;
		}

		[Test]
		public void DrawVertices()
		{
			RunCode =
				() => Resolve<Drawing>().DrawVertices(VerticesMode.Lines, new VertexPositionColor[4]);
			Window.CloseAfterFrame();
		}

		[Test]
		public void DrawVerticesWithIndices()
		{
			RunCode = () =>
			{
				var drawing = Resolve<Drawing>();
				drawing.SetIndices(new short[16], 16);
				drawing.DrawVertices(VerticesMode.Lines, new VertexPositionColor[4]);
			};
			Window.CloseAfterFrame();
		}

		[Test]
		public void SetIndicesTwice()
		{
			var drawing = Resolve<Drawing>();
			drawing.SetIndices(new short[16], 16);
			drawing.SetIndices(new short[16], 16);
			Window.CloseAfterFrame();
		}

		[Test]
		public void CheckCreatePositionColorBuffer()
		{
			var drawing = Resolve<Drawing>();
			drawing.DrawVertices(VerticesMode.Triangles, new VertexPositionColor[4]);
			drawing.DrawVertices(VerticesMode.Triangles, new VertexPositionColor[4]);
			Window.CloseAfterFrame();
		}

		[Test, ApproveFirstFrameScreenshot]
		public void ShowYellowLineFullscreen()
		{
			Settings.StartInFullscreen = true;
			Settings.Resolution = new Size(1920, 1080);
			var drawing = Resolve<Drawing>();
			RunCode = () => drawing.DrawVertices(VerticesMode.Lines, CreateLine(Color.Yellow));
			Settings.StartInFullscreen = false;
			Settings.Resolution = new Size(640, 360);
		}
	}
}