using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// Sets up an Entity that can be used in line rendering
	/// </summary>
	public class Line2D : Entity2D
	{
		public Line2D(Point start, Point end, Color color)
			: base(new Rectangle(start, (Size)(end - start)), color)
		{
			Add(new List<Point> { start, end });
			Add<Render2DLines>();
		}

		public List<Point> Points
		{
			get { return Get<List<Point>>(); }
			set { Set(value); }
		}

		public Point Start
		{
			get { return Points[0]; }
			set { Points[0] = value; }
		}

		public Point End
		{
			get
			{
				var points = Points;
				return points[points.Count - 1];
			}
			set
			{
				var points = Points;
				points[points.Count - 1] = value;
			}
		}

		public void AddLine(Point start, Point end)
		{
			var points = Points;
			points.Add(start);
			points.Add(end);
		}

		public void ExtendLine(Point nextPoint)
		{
			var points = Points;
			points.Add(points[points.Count - 1]);
			points.Add(nextPoint);
		}
	}
}