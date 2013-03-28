using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Shapes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Draws an unfilled rectangle in 2D space
	/// </summary>
	public class OutlinedRect : Rect
	{
		public OutlinedRect(Rectangle drawArea, Color color)
			: base(drawArea, color) {}

		public OutlinedRect(Point center, Size size, Color color)
			: base(center, size, color) {}

		protected override void Render(Renderer renderer, Time time)
		{
			renderer.DrawLine(DrawArea.TopLeft, DrawArea.TopRight, Color);
			renderer.DrawLine(DrawArea.TopRight, DrawArea.BottomRight, Color);
			renderer.DrawLine(DrawArea.BottomRight, DrawArea.BottomLeft, Color);
			renderer.DrawLine(DrawArea.BottomLeft, DrawArea.TopLeft, Color);
		}
	}
}
