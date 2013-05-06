using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// Renders a filled 2D ellipse shape
	/// </summary>
	public class Ellipse : Polygon
	{
		public Ellipse()
			: this(Rectangle.Zero, Color.White) {}

		public Ellipse(Rectangle drawArea, Color color)
			: base(color)
		{
			DrawArea = drawArea;
			Add<CalculateEllipsePoints>();
		}

		public Ellipse(Point center, float radiusX, float radiusY)
			: this(Rectangle.FromCenter(center, new Size(2 * radiusX, 2 * radiusY)), Color.White) {}

		public float RadiusX
		{
			get { return DrawArea.Width / 2.0f; }
			set
			{
				var drawArea = DrawArea;
				DrawArea = Rectangle.FromCenter(drawArea.Center, new Size(2 * value, drawArea.Height));
			}
		}

		public float RadiusY
		{
			get { return DrawArea.Height / 2.0f; }
			set
			{
				var drawArea = DrawArea;
				DrawArea = Rectangle.FromCenter(drawArea.Center, new Size(drawArea.Width, 2 * value));
			}
		}

		public float Radius
		{
			get { return MathExtensions.Max(RadiusX, RadiusY); }
			set
			{
				var drawArea = DrawArea;
				DrawArea = Rectangle.FromCenter(drawArea.Center, new Size(2 * value, 2 * value));
			}
		}
	}
}