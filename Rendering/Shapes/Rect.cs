using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// A rectangle to be rendered
	/// </summary>
	public class Rect : Polygon
	{
		public Rect()
			: this(Rectangle.Zero, Color.White) {}

		public Rect(Rectangle drawArea, Color color)
			: base(color)
		{
			DrawArea = drawArea;
			Add<CalculateRectCorners>();
		}
	}
}