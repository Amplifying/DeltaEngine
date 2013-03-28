using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace DeltaEngine.Multimedia
{
	public abstract class VideoSurface : Renderable
	{
		protected VideoSurface(Drawing drawing, Renderer renderer, Video video)
		{
			this.drawing = drawing;
			screen = renderer.Screen;
			this.video = video;
		}

		protected Video video;
		private readonly Drawing drawing;
		private readonly ScreenSpace screen;

		protected override void Render(Renderer renderer, Time time)
		{
			video.Run();

			var vertices = new[]
			{
				GetVertex(screen.TopLeft, Point.Zero), GetVertex(new Point(screen.Right, screen.Top), Point.UnitX),
				GetVertex(screen.BottomRight, Point.One), GetVertex(new Point(screen.Left, screen.Bottom), Point.UnitY)
			};

			drawing.SetIndices(QuadIndices, QuadIndices.Length);
			drawing.DrawVertices(VerticesMode.Triangles, vertices);
		}

		private VertexPositionColorTextured GetVertex(Point position, Point uv)
		{
			return new VertexPositionColorTextured(screen.ToPixelSpaceRounded(position), Color.White, uv);
		}

		private static readonly short[] QuadIndices = new short[] { 0, 1, 2, 0, 2, 3 };
	}
}
