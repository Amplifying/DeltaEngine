using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Fonts;

namespace DeltaEngine.Scenes.UserInterfaces.Graphing
{
	/// <summary>
	/// Renders the key to the graph lines below the graph
	/// </summary>
	public class RenderKey : EventListener2D
	{
		public RenderKey()
		{
			font = new Font("Verdana12");
		}

		private readonly Font font;

		public override void ReceiveMessage(Entity2D graph, object message)
		{
			if (!(message is ObserveEntity2D.HasChanged))
				return;

			var data = graph.Get<Graph.Data>();
			ClearOldKeyLabels(data);
			if (graph.Visibility == Visibility.Show)
				CreateNewKeyLabels(graph, data);
		}

		private void ClearOldKeyLabels(Graph.Data data)
		{
			foreach (FontText keyLabel in data.KeyLabels)
				SendFontTextToPool(keyLabel);

			data.KeyLabels.Clear();
		}

		private void SendFontTextToPool(FontText keyLabel)
		{
			keyLabel.Visibility = Visibility.Hide;
			fontTextPool.Add(keyLabel);
		}

		private readonly List<FontText> fontTextPool = new List<FontText>();

		private void CreateNewKeyLabels(Entity2D entity, Graph.Data data)
		{
			for (int i = 0; i < data.Lines.Count; i++)
				if (data.Lines[i].Key != "")
					CreateKeyLabel(entity, data, i);
		}

		private void CreateKeyLabel(Entity2D entity, Graph.Data data, int index)
		{
			var keyLabel = PullFontTextFromPool();
			int row = 1 + index / 6;
			float borderHeight = entity.DrawArea.Height * Graph.Border;
			float y = entity.DrawArea.Bottom + (4 * row) * borderHeight;
			float borderWidth = entity.DrawArea.Width * Graph.Border;
			float left = entity.DrawArea.Left + borderWidth;
			int column = index % 6;
			float interval = (entity.DrawArea.Width - 2 * borderWidth) / 6;
			float x = left + column * interval;
			keyLabel.Text = data.Lines[index].Key;
			keyLabel.SetPosition(new Point(x, y));
			keyLabel.RenderLayer = entity.RenderLayer + RenderLayerOffset;
			keyLabel.Color = data.Lines[index].Color;
			keyLabel.Visibility = Visibility.Show;
			data.KeyLabels.Add(keyLabel);
		}

		private FontText PullFontTextFromPool()
		{
			if (fontTextPool.Count == 0)
				return new FontText(font, "", Point.Zero);

			FontText keyLabel = fontTextPool[fontTextPool.Count - 1];
			fontTextPool.RemoveAt(fontTextPool.Count - 1);
			return keyLabel;
		}

		private const int RenderLayerOffset = 2;
	}
}