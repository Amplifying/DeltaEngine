using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;

namespace DeltaEngine.Scenes.UserInterfaces.Graphing
{
	/// <summary>
	/// Horizontal lines at fixed intervals - eg. if there were five percentiles there'd be 
	/// six lines at 0%, 20%, 40%, 60%, 80% and 100% of the maximum value
	/// </summary>
	public class RenderPercentiles : EventListener2D
	{
		public override void ReceiveMessage(Entity2D entity, object message)
		{
			if (!(message is ObserveEntity2D.HasChanged))
				return;

			var data = entity.Get<Graph.Data>();
			ClearOldPercentiles(data);
			if (entity.Visibility == Visibility.Show)
				CreateNewPercentiles(entity, data);
		}

		private void ClearOldPercentiles(Graph.Data data)
		{
			foreach (Line2D percentile in data.Percentiles)
			{
				percentile.Visibility = Visibility.Hide;
				line2DPool.Add(percentile);
			}

			data.Percentiles.Clear();
		}

		private readonly List<Line2D> line2DPool = new List<Line2D>();

		private void CreateNewPercentiles(Entity2D entity, Graph.Data data)
		{
			for (int i = 0; i <= data.NumberOfPercentiles; i++)
				CreatePercentile(entity, data, i);
		}

		private void CreatePercentile(Entity2D entity, Graph.Data data, int index)
		{
			Line2D percentile;
			if (line2DPool.Count > 0)
			{
				percentile = line2DPool[line2DPool.Count - 1];
				line2DPool.RemoveAt(line2DPool.Count - 1);
			}
			else
				percentile = new Line2D(Point.Zero, Point.Zero, Color.Black);

			float borderHeight = entity.DrawArea.Height * Graph.Border;
			float interval = (entity.DrawArea.Height - 2 * borderHeight) / data.NumberOfPercentiles;
			float bottom = entity.DrawArea.Bottom - borderHeight;
			float y = bottom - index * interval;
			float borderWidth = entity.DrawArea.Width * Graph.Border;
			float startX = entity.DrawArea.Left + borderWidth;
			float endX = entity.DrawArea.Right - borderWidth;
			percentile.StartPoint = new Point(startX, y);
			percentile.EndPoint = new Point(endX, y);
			percentile.Color = data.PercentileColor;
			percentile.RenderLayer = entity.RenderLayer + RenderLayerOffset;
			percentile.Visibility = Visibility.Show;
			data.Percentiles.Add(percentile);
		}

		private const int RenderLayerOffset = 1;
	}
}