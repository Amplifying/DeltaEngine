using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Draws an unfilled circle in 2D space
	/// </summary>
	public class Circle : Renderable
	{
		public Circle(Point center, float radius, Color color)
			: this(center, radius)
		{
			Color = color;
		}

		public Color Color = Color.White;

		public Circle(Point center, float radius)
		{
			Center = center;
			Radius = radius;
		}

		public Point Center;

		public float Radius
		{
			get { return radius; }
			set
			{
				if (radius == value)
					return;

				radius = value;
				StoreCirclePoints();
			}
		}

		private float radius;

		private void StoreCirclePoints()
		{
			var pointsCount = (int)(MaxPoints * MathExtensions.Max(0.22f + radius / 2, radius));
			pointsCount = (int)MathExtensions.Max(pointsCount, MinPoints);
			float theta = 360.0f / (pointsCount - 1);
			circlePoints = new Point[pointsCount];

			for (int i = 0; i < pointsCount; i++)
				circlePoints[i] = radius *
					new Point(fastTrig.Sin(i * theta), fastTrig.Cos(i * theta));
		}

		public int MinPoints { get; set; }
		public const int MaxPoints = 96;
		protected Point[] circlePoints;
		private readonly FastTrig fastTrig = new FastTrig();

		protected override void Render(Renderer renderer, Time time)
		{
			for (int i = 1; i < circlePoints.Length; i++)
				renderer.DrawLine(Center + circlePoints[i - 1], Center + circlePoints[i], Color);
		}
	}
}