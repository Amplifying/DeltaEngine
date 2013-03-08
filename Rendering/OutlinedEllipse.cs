using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Shapes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Draws an unfilled ellipse in 2D space
	/// </summary>
	public class OutlinedEllipse : Ellipse
	{
		public OutlinedEllipse(Point center, float radiusX, float radiusY)
			: base(center, radiusX, radiusY) {}

		protected override void Render(Renderer renderer, Time time)
		{
			if (areEllipsePointsOutOfDate)
				StoreEllipsePoints();

			for (int i = 1; i < ellipsePoints.Length; i++)
				renderer.DrawLine(Center + ellipsePoints[i - 1], Center + ellipsePoints[i], Color);
		}
	}
}