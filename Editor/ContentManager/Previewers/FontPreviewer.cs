using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Fonts;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class FontPreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			var font = ContentLoader.Load<Font>(contentName);
			if (currentDisplayText != null)
				currentDisplayText.IsActive = false;
			currentDisplayText = new FontText(font, PreviewText,
				Rectangle.FromCenter(Point.Half, new Size(1, 1)));
			SetFontViewCommands();
		}

		private void SetFontViewCommands()
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(MoveFontText).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
		}

		private Point lastPanPosition = Point.Unused;
		public FontText currentDisplayText;

		private void MoveFontText(Point mousePosition)
		{
			var relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			currentDisplayText.Center += relativePosition;
		}

		private const string PreviewText = "The quick brown fox jumps over the lazy dog";
	}
}