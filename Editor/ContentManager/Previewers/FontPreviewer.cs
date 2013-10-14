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
			ContentDisplayChanger.SetEntity2DMoveCommand(currentDisplayText);
		}

		public FontText currentDisplayText;
		private const string PreviewText = "The quick brown fox jumps over the lazy dog";
	}
}