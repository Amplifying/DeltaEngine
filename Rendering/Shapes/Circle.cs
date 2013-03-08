using DeltaEngine.Core;
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
			: base(center, radius, radius)
		{}

		public float Radius
		{
			get { return RadiusX; }
			set
			{
				if (RadiusX == value)
					return;

				RadiusX = value;
				StoreCirclePoints();
			}
		}

		private void StoreCirclePoints()
		{
			var pointsCount = GetPointsCount(Radius);
			var theta = CalculateTheta(pointsCount);

			for (int i = 0; i < pointsCount; i++)
				ellipsePoints[i] = Radius *
					new Point(MathExtensions.Sin(i * theta), MathExtensions.Cos(i * theta));
		}
	}
}