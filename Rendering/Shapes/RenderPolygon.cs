using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// Responsible for rendering filled 2D shapes defined by their border points
	/// </summary>
	public class RenderPolygon : EntityListener
	{
		public RenderPolygon(Drawing draw, ScreenSpace screen)
		{
			this.draw = draw;
			this.screen = screen;
		}

		private readonly Drawing draw;
		private readonly ScreenSpace screen;

		public void Handle(List<Entity> entities) {}

		public void ReceiveMessage(Entity entity, object message)
		{
			if (message is SortAndRenderEntity2D.TimeToRender)
				Render(entity);
		}

		private void Render(Entity entity)
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

		public EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.Normal; }
		}
	}
}