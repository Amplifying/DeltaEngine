using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// Responsible for rendering the outline of 2D shapes defined by their border points
	/// </summary>
	public class DrawPolygon2DOutlines : DrawBehavior
	{
		public DrawPolygon2DOutlines(Drawing draw)
		{
			this.draw = draw;
			material = new Material(Shader.Position2DColor, "");
		}

		private readonly Drawing draw;
		private readonly Material material;

		public void Draw(IEnumerable<DrawableEntity> entities)
		{
			foreach (var entity in entities)
				AddToBatch(entity);
			if (vertices.Count == 0)
				return;
			draw.AddLines(material, vertices.ToArray());
			vertices.Clear();
		}

		private void AddToBatch(DrawableEntity entity)
		{
			var color = entity.Get<OutlineColor>().Value;
			List<Point> points = null;
			if (entity.Contains<List<Point>>())
				points = entity.Get<List<Point>>();
			else if (entity is Entity2D)
			{
				points = new List<Point>();
				var drawArea = (entity as Entity2D).DrawArea;
				points.Add(drawArea.TopLeft);
				points.Add(drawArea.TopRight);
				points.Add(drawArea.BottomRight);
				points.Add(drawArea.BottomLeft);
			}
			if (points == null || points.Count <= 1)
				return;
			lastPoint = points[points.Count - 1];
			foreach (Point point in points)
				AddLine(point, color);
		}

		private Point lastPoint;

		private void AddLine(Point point, Color color)
		{
			vertices.Add(new VertexPosition2DColor(ScreenSpace.Current.ToPixelSpaceRounded(lastPoint),
				color));
			vertices.Add(new VertexPosition2DColor(ScreenSpace.Current.ToPixelSpaceRounded(point), color));
			lastPoint = point;
		}

		private readonly List<VertexPosition2DColor> vertices = new List<VertexPosition2DColor>();
	}
}