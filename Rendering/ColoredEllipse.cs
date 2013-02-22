using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Draws a filled ellipse in 2D space, with the border optionally a different color
	/// </summary>
	public class ColoredEllipse : Ellipse
	{
		public ColoredEllipse(Point center, float radiusX, float radiusY)
			: base(center, radiusX, radiusY) {}

		protected override void Render(Renderer renderer, Time time)
		{
			if (areEllipsePointsOutOfDate)
				StoreEllipsePoints();

			for (int i = 1; i < ellipsePoints.Length; i++)
				renderer.DrawTriangle(
					new Triangle2D(Center, Center + ellipsePoints[i - 1], Center + ellipsePoints[i]), Color);

			if (BorderColor != Color)
				for (int i = 1; i < ellipsePoints.Length; i++)
					renderer.DrawLine(Center + ellipsePoints[i - 1], Center + ellipsePoints[i], BorderColor);
		}

		public Color BorderColor;
	}
}