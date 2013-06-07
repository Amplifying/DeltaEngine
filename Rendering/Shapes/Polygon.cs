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
				vertices = new List<VertexPositionColor>();
			}

			private readonly Drawing draw;
			private readonly ScreenSpace screen;
			private readonly List<VertexPositionColor> vertices;

			public override void Handle(Entity entity)
			{
				if (entity.Get<Visibility>() == Visibility.Hide)
					return;

				vertices.Clear();
				RenderPolygon(entity);
				SetIndices();
			}

			public override void ReceiveMessage(Entity entity, object message) {}

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
				SetTrianglePoints(new Triangle2D(center, point, lastPoint), color);
				lastPoint = point;
			}

			private void SetTrianglePoints(Triangle2D triangle, Color color)
			{
				vertices.Add(new VertexPositionColor(screen.ToPixelSpaceRounded(triangle.Corner1), color));
				vertices.Add(new VertexPositionColor(screen.ToPixelSpaceRounded(triangle.Corner2), color));
				vertices.Add(new VertexPositionColor(screen.ToPixelSpaceRounded(triangle.Corner3), color));
			}

			private static short[] triangleIndices = { 0, 1, 2 };

			private void SetIndices()
			{
				var newVertices = new VertexPositionColor[vertices.Count + 1];
				var newIndices = new short[vertices.Count + 1];
				for (int posInList = 0; posInList < vertices.Count; ++posInList)
					NumberOfVertex(newVertices, posInList, newIndices);

				triangleIndices = newIndices;
				DrawPolygon(newVertices);
			}

			private void DrawPolygon(VertexPositionColor[] newVertices)
			{
				draw.DisableTexturing();
				draw.SetIndices(triangleIndices, triangleIndices.Length);
				draw.DrawVertices(VerticesMode.Triangles, newVertices);
			}

			private void NumberOfVertex(VertexPositionColor[] newVertices, int posInList,
				short[] newIndices)
			{
				newVertices[posInList] = vertices[posInList];
				newIndices[posInList] = (short)posInList;
			}
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
				vertices = new List<VertexPositionColor>();
			}

			private readonly Drawing draw;
			private readonly ScreenSpace screen;
			private readonly List<VertexPositionColor> vertices;

			public override void Handle(Entity entity)
			{
				vertices.Clear();
				RenderPolygonOutline(entity);
				draw.DrawVertices(VerticesMode.Lines, vertices.ToArray());
			}

			public override void ReceiveMessage(Entity entity, object message) {}

			private void RenderPolygonOutline(Entity entity)
			{
				var color = entity.Get<OutlineColor>().Value;
				var points = entity.Get<List<Point>>();
				if (points.Count == 0)
					return;

				lastPoint = points[points.Count - 1];
				foreach (Point point in points)
					AddLine(point, color);
			}

			private Point lastPoint;

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