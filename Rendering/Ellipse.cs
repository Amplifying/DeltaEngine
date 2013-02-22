using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Draws an unfilled ellipse in 2D space
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
			if (areEllipsePointsOutOfDate)
				StoreEllipsePoints();

			for (int i = 1; i < ellipsePoints.Length; i++)
				renderer.DrawLine(Center + ellipsePoints[i - 1], Center + ellipsePoints[i], Color);
		}

		protected void StoreEllipsePoints()
		{
			FormRotationSinCos();
			FormEllipsePoints();
			areEllipsePointsOutOfDate = false;
		}

		private void FormRotationSinCos()
		{
			rotationSin = fastTrig.Sin(rotation);
			rotationCos = fastTrig.Cos(rotation);
		}

		private float rotationSin;
		private float rotationCos;
		protected readonly FastTrig fastTrig = new FastTrig();

		private void FormEllipsePoints()
		{
			float maxRadius = MathExtensions.Max(radiusX, radiusY);
			var pointsCount = (int)(MaxPoints * MathExtensions.Max(0.22f + maxRadius / 2, maxRadius));
			pointsCount = (int)MathExtensions.Max(pointsCount, MinPoints);

			theta = 360.0f / (pointsCount - 1);
			ellipsePoints = new Point[pointsCount];

			for (int i = 0; i < pointsCount; i++)
				FormRotatedEllipsePoint(i);
		}

		public int MinPoints { get; set; }
		public const int MaxPoints = 96;
		private float theta;

		private void FormRotatedEllipsePoint(int i)
		{
			var ellipsePoint = new Point(radiusX * fastTrig.Sin(i * theta),
				radiusY * fastTrig.Cos(i * theta));
			ellipsePoint.RotateAround(Point.Zero, rotationSin, rotationCos);
			ellipsePoints[i] = ellipsePoint;
		}

		protected Point[] ellipsePoints;
		public Color Color = Color.White;
	}
}