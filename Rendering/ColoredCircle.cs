using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Draws a filled circle in 2D space, with the border optionally a different color
	/// </summary>
	public class ColoredCircle : Circle
	{
		public ColoredCircle(Point center, float radius, Color color)
			: base(center, radius, color) {}

		public ColoredCircle(Point center, float radius)
			: base(center, radius) {}

		protected override void Render(Renderer renderer, Time time)
		{
			for (int i = 1; i < circlePoints.Length; i++)
				renderer.DrawTriangle(
					new Triangle2D(Center, Center + circlePoints[i - 1], Center + circlePoints[i]), Color);

			if (BorderColor != Color)
				for (int i = 1; i < circlePoints.Length; i++)
					renderer.DrawLine(Center + circlePoints[i - 1], Center + circlePoints[i], BorderColor);
		}

		public Color BorderColor;
	}
}