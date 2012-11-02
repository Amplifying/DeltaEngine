using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Keeps position, size and color for an automatically rendered rectangle on screen.
	/// </summary>
	public class ColoredRectangle : Renderable
	{
		public ColoredRectangle(Renderer render, Rectangle rect, Color color)
			: base(render)
		{
			Rect = rect;
			Color = color;
		}

		public ColoredRectangle(Renderer render, Point center, Size size, Color color)
			: this(render, Rectangle.FromCenter(center, size), color) { }

		public Rectangle Rect;
		public Color Color;

		protected internal override void Render()
		{
			renderer.DrawRectangle(Rect, Color);
		}
	}
}