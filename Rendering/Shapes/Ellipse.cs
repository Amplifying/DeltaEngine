using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// Draws a filled ellipse in 2D space, with the border optionally a different color
	/// </summary>
	public class Ellipse : Renderable
	{
		public Ellipse(Point center, float radiusX, float radiusY)
		{
			Center = center;
			this.radiusX = radiusX;
			this.radiusY = radiusY;
		}

		public Point Center;

		public float RadiusX
		{
			get { return radiusX; }
			set
			{
				if (radiusX == value)
					return;

				radiusX = value;
				areEllipsePointsOutOfDate = true;
			}
		}

		private float radiusX;
		protected bool areEllipsePointsOutOfDate = true;

		public float RadiusY
		{
			get { return radiusY; }
			set
			{
				if (radiusY == value)
					return;

				radiusY = value;
				areEllipsePointsOutOfDate = true;
			}
		}

		private float radiusY;

		public float MaxRadius
		{
			get { return MathExtensions.Max(radiusX, radiusY); }
		}

		public float Rotation
		{
			get { return rotation; }
			set
			{
				if (rotation == value)
					return;

				rotation = value;
				areEllipsePointsOutOfDate = true;
			}
		}

		private float rotation;

		protected override void Render(Renderer renderer, Time time)
		{
			AreEllipsePointsOutOfDate();

			DrawInnerEllipse(renderer);

			DrawBorderEllipse(renderer);
		}

		private void AreEllipsePointsOutOfDate()
		{
			if (areEllipsePointsOutOfDate)
				StoreEllipsePoints();
		}

		private void DrawInnerEllipse(Renderer renderer)
		{
			for (int i = 1; i < ellipsePoints.Length; i++)
				renderer.DrawTriangle(
					new Triangle2D(Center, Center + ellipsePoints[i - 1], Center + ellipsePoints[i]), Color);
		}

		private void DrawBorderEllipse(Renderer renderer)
		{
			if (BorderColor != Color)
				for (int i = 1; i < ellipsePoints.Length; i++)
					renderer.DrawLine(Center + ellipsePoints[i - 1], Center + ellipsePoints[i], BorderColor);
		}

		public Color BorderColor;

		protected void StoreEllipsePoints()
		{
			StoreRotationSinCos();
			FormEllipsePoints();
			areEllipsePointsOutOfDate = false;
		}

		private void StoreRotationSinCos()
		{
			rotationSin = MathExtensions.Sin(rotation);
			rotationCos = MathExtensions.Cos(rotation);
		}

		private float rotationSin;
		private float rotationCos;

		private void FormEllipsePoints()
		{
			float maxRadius = MathExtensions.Max(radiusX, radiusY);
			var pointsCount = GetPointsCount(maxRadius);

			theta = CalculateTheta(pointsCount);

			for (int i = 0; i < pointsCount; i++)
				FormRotatedEllipsePoint(i);
		}

		private float theta;

		protected int GetPointsCount(float maxRadius)
		{
			var pointsCount = (int)(MaxPoints * MathExtensions.Max(0.22f + maxRadius / 2, maxRadius));
			return MathExtensions.Max(pointsCount, MinPoints);
		}

		private const int MinPoints = 5;
		private const int MaxPoints = 96;

		protected float CalculateTheta(int pointsCount)
		{
			float thetaFloat = 360.0f / (pointsCount - 1);
			ellipsePoints = new Point[pointsCount];
			return thetaFloat;
		}

		private void FormRotatedEllipsePoint(int i)
		{
			var ellipsePoint = new Point(radiusX * MathExtensions.Sin(i * theta),
				radiusY * MathExtensions.Cos(i * theta));
			ellipsePoint.RotateAround(Point.Zero, rotationSin, rotationCos);
			ellipsePoints[i] = ellipsePoint;
		}

		protected Point[] ellipsePoints;
		public Color Color = Color.White;
	}
}