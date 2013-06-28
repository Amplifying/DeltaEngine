using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Shapes;

namespace DeltaEngine.Scenes.UserInterfaces.Terminal
{
	public sealed class Terminal : FilledRect
	{
		public Terminal(Window window)
			: base(Rectangle.Zero, BackgroundColor)
		{
			screenSpace = new QuadraticScreenSpace(window);
			fontHeight = screenSpace.FromPixelSpace(new Size(FontPixelSize)).Height;
			history = new History(fontHeight);
			terminalInput = new TextLine("> ", fontHeight);
			UpdateDrawArea();
			window.ViewportSizeChanged += size => UpdateDrawArea();
			Start<InteractWithKeyboard>().Messaged += KeyboardEvent;
		}

		private static readonly Color BackgroundColor = new Color(0, 167, 255, 0.5f);
		public const float FontPixelSize = 12.0f;
		private readonly float fontHeight;
		private readonly ScreenSpace screenSpace;
		private readonly History history;
		private readonly TextLine terminalInput;

		private void UpdateDrawArea()
		{
			DrawArea = GetDrawArea();
			terminalInput.Position = DrawArea.TopLeft;
			history.Position = new Point(DrawArea.Left + HistoryMargin, DrawArea.Top + fontHeight);
		}

		private const float HistoryMargin = 0.1f;

		private Rectangle GetDrawArea()
		{
			return new Rectangle
			{
				Left = screenSpace.Viewport.Left + LeftMargin,
				Top = screenSpace.Viewport.Top + TopMargin,
				Width = screenSpace.Viewport.Width - (LeftMargin + RightMargin),
				Height = (history.history.Count + 3) * fontHeight
			};
		}

		private const float LeftMargin = 0.01f;
		private const float RightMargin = 0.01f;
		private const float TopMargin = 0.1f;

		private void KeyboardEvent(object message)
		{
			var keyPressed = message as InteractWithKeyboard.KeyPress;
			if (keyPressed == null)
				return;

			if (keyPressed.Key == Key.Enter)
				MoveTerminalInputTextToNextLine();
			else
				terminalInput.Text += keyPressed.Key;
		}

		private void MoveTerminalInputTextToNextLine()
		{
			history.AddLine(terminalInput.Text.Substring(2));
			terminalInput.Text = "> ";
			UpdateDrawArea();
		}
	}
}