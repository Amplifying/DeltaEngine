using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Draws line in 2D space.
	/// </summary>
	public class Line2D : Renderable
	{
		public Line2D(Point startPosition, Point endPosition, Color color)
		{
			StartPosition = startPosition;
			EndPosition = endPosition;
			Color = color;
		}

		public Point StartPosition;
		public Point EndPosition;
		public Color Color;

		protected override void Render(Renderer renderer, Time time)
		{
			renderer.DrawLine(StartPosition, EndPosition, Color);
		}
	}
}