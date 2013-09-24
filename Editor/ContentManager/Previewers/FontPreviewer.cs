using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D.Fonts;

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
				Rectangle.FromCenter(Vector2D.Half, new Size(1, 1)));
			SetFontViewCommands();
		}

		private void SetFontViewCommands()
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(MoveFontText).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
		}

		private Vector2D lastPanPosition = Vector2D.Unused;
		public FontText currentDisplayText;

		private void MoveFontText(Vector2D mousePosition)
		{
			var relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			currentDisplayText.Center += relativePosition;
		}

		private const string PreviewText = "The quick brown fox jumps over the lazy dog";
	}
}