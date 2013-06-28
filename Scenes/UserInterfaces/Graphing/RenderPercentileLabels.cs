using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Fonts;

namespace DeltaEngine.Scenes.UserInterfaces.Graphing
{
	/// <summary>
	/// Labels at fixed vertical intervals to the right of the graph - eg. if there were 
	/// five percentiles there'd be six lines at 0%, 20%, 40%, 60%, 80% and 100% of the 
	/// maximum value
	/// </summary>
	public class RenderPercentileLabels : EventListener2D
	{
		public RenderPercentileLabels()
		{
			font = new Font("Verdana12");
		}

		private readonly Font font;

		public override void ReceiveMessage(Entity2D entity, object message)
		{
			if (!(message is ObserveEntity2D.HasChanged))
				return;

			var data = entity.Get<Graph.Data>();
			ClearOldPercentileLabels(data);
			if (entity.Visibility == Visibility.Show)
				CreateNewPercentileLabels(entity, data);
		}

		private void ClearOldPercentileLabels(Graph.Data data)
		{
			foreach (FontText percentileLabel in data.PercentileLabels)
				SendFontTextToPool(percentileLabel);

			data.PercentileLabels.Clear();
		}

		private void SendFontTextToPool(FontText percentileLabel)
		{
			percentileLabel.Visibility = Visibility.Hide;
			fontTextPool.Add(percentileLabel);
		}

		private readonly List<FontText> fontTextPool = new List<FontText>();

		private void CreateNewPercentileLabels(Entity2D entity, Graph.Data data)
		{
			for (int i = 0; i <= data.NumberOfPercentiles; i++)
				CreatePercentileLabel(entity, data, i);
		}

		private void CreatePercentileLabel(Entity2D entity, Graph.Data data, int index)
		{
			var percentileLabel = PullFontTextFromPool();
			float borderHeight = entity.DrawArea.Height * Graph.Border;
			float interval = (entity.DrawArea.Height - 2 * borderHeight) / data.NumberOfPercentiles;
			float bottom = entity.DrawArea.Bottom - borderHeight;
			float y = bottom - index * interval;
			float borderWidth = entity.DrawArea.Width * Graph.Border;
			float x = entity.DrawArea.Right + 2 * borderWidth;
			float value = data.Viewport.Top + index * data.Viewport.Height / data.NumberOfPercentiles;
			if (data.ArePercentileLabelsInteger)
				value = (int)value;

			percentileLabel.Text = data.PercentilePrefix + value + data.PercentileSuffix;
			percentileLabel.SetPosition(new Point(x, y));
			percentileLabel.RenderLayer = entity.RenderLayer + RenderLayerOffset;
			percentileLabel.Color = data.PercentileLabelColor;
			percentileLabel.Visibility = Visibility.Show;
			data.PercentileLabels.Add(percentileLabel);
		}

		private FontText PullFontTextFromPool()
		{
			if (fontTextPool.Count == 0)
				return new FontText(font, "", Point.Zero);

			FontText percentileLabel = fontTextPool[fontTextPool.Count - 1];
			fontTextPool.RemoveAt(fontTextPool.Count - 1);
			return percentileLabel;
		}

		private const int RenderLayerOffset = 2;
	}
}