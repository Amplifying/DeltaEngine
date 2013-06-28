using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Scenes.UserInterfaces.Terminal
{
	public class History
	{
		public History(float fontHeight)
		{
			this.fontHeight = fontHeight;
			history = new List<TextLine>();
		}

		private readonly float fontHeight;
		internal readonly List<TextLine> history;

		public void AddLine(string text)
		{
			CheckMaxHistoryCount();
			history.Add(new TextLine(text, fontHeight));
			UpdateHistoryLinePosition(history.Count - 1);
		}

		private void CheckMaxHistoryCount()
		{
			if (history.Count < MaxHistoryCount)
				return;

			history[0].Dispose();
			history.RemoveAt(0);
		}

		private const int MaxHistoryCount = 20;

		private void UpdateHistoryLinePosition(int index)
		{
			history[index].Position = new Point(position.X, position.Y + (index + 1) * fontHeight);
		}

		public Point Position
		{
			get { return position; }
			set
			{
				position = value;
				for (int i = 0; i < history.Count; i++)
					UpdateHistoryLinePosition(i);
			}
		}

		private Point position;
	}
}