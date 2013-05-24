using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// A shape to be rendered defined by its border points, will be rendered with a filled color.
	/// </summary>
	public class Polygon : Entity2D
	{
		public Polygon()
			: this(Color.White) {}

		public Polygon(Color color)
			: base(Rectangle.Zero, color)
		{
			Add(new List<Point>());
			Add<Render>();
		}

		public List<Point> Points
		{
			get { return Get<List<Point>>(); }
		}

		/// <summary>
		/// Responsible for rendering filled 2D shapes defined by their border points
		/// </summary>
		public class Render : EntityListener
		{
			public Render(Drawing draw, ScreenSpace screen)
			{
				this.draw = draw;
				this.screen = screen;
			}

			private readonly Drawing draw;
			private readonly ScreenSpace screen;

			public override void ReceiveMessage(Entity entity, object message)
			{
				if (message is SortAndRender.TimeToRender)
					RenderPolygon(entity);
			}

			private void RenderPolygon(Entity entity)
			{
				var points = entity.Get<List<Point>>();
				if (points.Count < 3)
					return;

				var color = entity.Get<Color>();
				var center = GetCenter(points);
				lastPoint = points[points.Count - 1];
				foreach (Point point in points)
					CreateAndDrawTriangle(point, center, color);
			}

			private static Point GetCenter(ICollection<Point> points)
			{
				Point center = points.Aggregate(Point.Zero, (current, point) => current + point);
				return center / points.Count;
			}

			private Point lastPoint;

			private void CreateAndDrawTriangle(Point point, Point center, Color color)
			{
				DrawTriangle(new Triangle2D(center, point, lastPoint), color);
				lastPoint = point;
			}

			private void DrawTriangle(Triangle2D triangle, Color color)
			{
				draw.DisableTexturing();
				draw.SetIndices(TriangleIndices, TriangleIndices.Length);
				var vertices = new[]
				{
					new VertexPositionColor(screen.ToPixelSpaceRounded(triangle.Corner1), color),
					new VertexPositionColor(screen.ToPixelSpaceRounded(triangle.Corner2), color),
					new VertexPositionColor(screen.ToPixelSpaceRounded(triangle.Corner3), color)
				};
				draw.DrawVertices(VerticesMode.Triangles, vertices);
			}

			private static readonly short[] TriangleIndices = { 0, 1, 2 };
		}

		/// <summary>
		/// Responsible for rendering the outline of 2D shapes defined by their border points
		/// </summary>
		public class RenderOutline : EntityListener
		{
			public RenderOutline(Drawing draw, ScreenSpace screen)
			{
				this.draw = draw;
				this.screen = screen;
			}

			private readonly Drawing draw;
			private readonly ScreenSpace screen;

			public override void ReceiveMessage(Entity entity, object message)
			{
				if (message is SortAndRender.TimeToRender)
					RenderPolygonOutline(entity);
			}

			private void RenderPolygonOutline(Entity entity)
			{
				var color = entity.Get<OutlineColor>().Value;
				var points = entity.Get<List<Point>>();
				if (points.Count == 0)
					return;

				lastPoint = points[points.Count - 1];
				vertices = new List<VertexPositionColor>();
				foreach (Point point in points)
					AddLine(point, color);

				draw.DrawVertices(VerticesMode.Lines, vertices.ToArray());
			}

			private Point lastPoint;
			private List<VertexPositionColor> vertices;

			private void AddLine(Point point, Color color)
			{
				vertices.Add(new VertexPositionColor(screen.ToPixelSpaceRounded(lastPoint), color));
				vertices.Add(new VertexPositionColor(screen.ToPixelSpaceRounded(point), color));
				lastPoint = point;
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.Last; }
			}
		}
	}
}