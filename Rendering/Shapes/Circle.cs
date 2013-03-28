using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// Draws a filled circle in 2D space, with the border optionally a different color
	/// </summary>
	public class Circle : Ellipse
	{
		public Circle(Point center, float radius, Color color)
			: this(center, radius)
		{
			Color = color;
		}

		public Circle(Point center, float radius)
			: base(center, radius, radius) {}

		public float Radius
		{
			get { return (RadiusX + RadiusY) / 2; }
			set
			{
				RadiusX = value;
				RadiusY = value;
			}
		}
	}
}