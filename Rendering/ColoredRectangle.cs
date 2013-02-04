using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Keeps position, size and color for an automatically rendered rectangle on screen.
	/// </summary>
	public class ColoredRectangle : Renderable
	{
		public ColoredRectangle(Rectangle drawArea, Color color)
		{
			DrawArea = drawArea;
			Color = color;
		}

		public Rectangle DrawArea;
		public Color Color;

		public ColoredRectangle(Point center, Size size, Color color)
			: this(Rectangle.FromCenter(center, size), color) {}

		protected override void Render(Renderer renderer, Time time)
		{
			renderer.DrawRectangle(DrawArea, Color);
		}
	}
}