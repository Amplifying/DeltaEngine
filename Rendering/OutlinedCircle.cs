using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Draws an unfilled circle in 2D space
	/// </summary>
	public class OutlinedCircle : OutlinedEllipse
	{
		public OutlinedCircle(Point center, float radius, Color color)
			: base(center, radius, radius)
		{
			Color = color;
		}

		public OutlinedCircle(Point center, float radius)
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