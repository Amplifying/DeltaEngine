using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// Responsible for rendering the outline of 2D shapes defined by their border points
	/// </summary>
	public class RenderPolygonOutline : EntityListener
	{
		public RenderPolygonOutline(Drawing draw, ScreenSpace screen)
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

		public EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.Last; }
		}
	}
}