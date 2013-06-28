using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;

namespace DeltaEngine.Scenes.UserInterfaces.Graphing
{
	/// <summary>
	/// A line on a graph: consists of a list of points and a list of lines connecting them
	/// </summary>
	public class GraphLine
	{
		internal GraphLine(Graph graph)
		{
			this.graph = graph;
		}

		internal readonly Graph graph;

		public void AddValue(float value)
		{
			AddValue(1.0f, value);
		}

		public void AddValue(float interval, float value)
		{
			float x = points.Count == 0 ? -interval : points[points.Count - 1].X;
			AddPoint(new Point(x + interval, value));
		}

		public void AddPoint(Point point)
		{
			viewport = graph.Viewport;
			drawArea = graph.DrawArea;
			clippingBounds = Rectangle.FromCorners(
				ToQuadratic(viewport.BottomLeft, viewport, drawArea),
				ToQuadratic(viewport.TopRight, viewport, drawArea));
			InsertPointIntoSequence(point);
			graph.AddPoint(point);
		}

		private Rectangle viewport;
		private Rectangle clippingBounds;
		private Rectangle drawArea;

		private static Point ToQuadratic(Point point, Rectangle viewport, Rectangle drawArea)
		{
			float borderWidth = viewport.Width * Graph.Border;
			float borderHeight = viewport.Height * Graph.Border;
			float x = (point.X - viewport.Left + borderWidth) / (viewport.Width + 2 * borderWidth);
			float y = (point.Y - viewport.Top + borderHeight) / (viewport.Height + 2 * borderHeight);
			return new Point(drawArea.Left + x * drawArea.Width, drawArea.Bottom - y * drawArea.Height);
		}

		private void InsertPointIntoSequence(Point point)
		{
			if (points.Count == 0 || points[points.Count - 1].X <= point.X)
				AddPointToEnd(point);
			else
				AddPointToMiddle(point);
		}

		private void AddPointToEnd(Point point)
		{
			points.Add(point);
			if (points.Count <= 1)
				return;

			var line = new Line2D(ToQuadratic(points[points.Count - 2], viewport, drawArea),
				ToQuadratic(point, viewport, drawArea), Color);
			line.Clip(clippingBounds);
			lines.Add(line);
		}

		private void AddPointToMiddle(Point point)
		{
			for (int index = 0; index < points.Count; index++)
				if (points[index].X > point.X)
				{
					InsertPointAt(point, index);
					return;
				}
		}

		internal readonly List<Point> points = new List<Point>();

		private void InsertPointAt(Point point, int index)
		{
			if (index > 0)
				MoveLineEndpoint(point, index);

			var line = new Line2D(ToQuadratic(point, viewport, drawArea),
				ToQuadratic(points[index], viewport, drawArea), Color);
			line.Clip(clippingBounds);
			lines.Insert(index, line);
			points.Insert(index, point);
		}

		public Color Color
		{
			get { return color; }
			set
			{
				color = value;
				graph.Refresh();
			}
		}

		private Color color = Color.Blue;

		private void MoveLineEndpoint(Point point, int index)
		{
			Line2D line = lines[index - 1];
			line.EndPoint = ToQuadratic(point, viewport, drawArea);
			line.Clip(clippingBounds);
		}

		internal readonly List<Line2D> lines = new List<Line2D>();

		public void RemoveAt(int index)
		{
			if (index > 0 && index < points.Count - 1)
				lines[index - 1].EndPoint = lines[index].EndPoint;

			if (index <= lines.Count - 1)
				RemoveLine(index);
			else
				RemoveLine(index - 1);

			points.RemoveAt(index);
		}

		private void RemoveLine(int index)
		{
			lines[index].IsActive = false;
			lines.RemoveAt(index);
		}

		public void Clear()
		{
			points.Clear();
			ClearGraphics();
		}

		internal void ClearGraphics()
		{
			foreach (Line2D line in lines)
				SendLineToPool(line);

			lines.Clear();
		}

		private void SendLineToPool(Line2D line)
		{
			line.Visibility = Visibility.Hide;
			line2DPool.Add(line);
		}

		private readonly List<Line2D> line2DPool = new List<Line2D>();

		public void Refresh()
		{
			ClearGraphics();
			viewport = graph.Viewport;
			drawArea = graph.DrawArea;
			clippingBounds = Rectangle.FromCorners(
				ToQuadratic(viewport.BottomLeft, viewport, drawArea),
				ToQuadratic(viewport.TopRight, viewport, drawArea));
			for (int i = 1; i < points.Count; i++)
				CreateLine(i);
		}

		private void CreateLine(int i)
		{
			Line2D line = PullLineFromPool();
			line.StartPoint = ToQuadratic(points[i - 1], viewport, drawArea);
			line.EndPoint = ToQuadratic(points[i], viewport, drawArea);
			line.Color = Color;
			line.RenderLayer = graph.RenderLayer + RenderLayerOffset;
			line.Visibility = Visibility.Show;
			line.Clip(clippingBounds);
			lines.Add(line);
		}

		private Line2D PullLineFromPool()
		{
			if (line2DPool.Count == 0)
				return new Line2D(Point.Zero, Point.Zero, Color.Black);

			Line2D line = line2DPool[line2DPool.Count - 1];
			line2DPool.RemoveAt(line2DPool.Count - 1);
			return line;
		}

		private const int RenderLayerOffset = 3;

		public string Key
		{
			get { return key; }
			set
			{
				key = value;
				graph.Refresh();
			}
		}

		private string key = "";
	}
}