using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D.Sprites;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class ImagePreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			var image = ContentLoader.Load<Image>(contentName);
			var imageSize = image.PixelSize;
			var aspectRatio = imageSize.Height / imageSize.Width;
			if (currentDisplaySprite != null)
				currentDisplaySprite.IsActive = false;
			currentDisplaySprite = new Sprite(new Material(Shader.Position2DUv, contentName),
				Rectangle.FromCenter(new Vector2D(0.5f, 0.5f), new Size(0.5f, 0.5f * aspectRatio)));
			SetImageCommands();
		}

		public Sprite currentDisplaySprite;

		private void SetImageCommands()
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(MoveImage).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
			new Command(position => lastScalePosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
			new Command(ScaleImage).Add(new MousePositionTrigger(MouseButton.Middle, State.Pressed));
		}

		private Vector2D lastPanPosition = Vector2D.Unused;

		public void MoveImage(Vector2D mousePosition)
		{
			var relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			currentDisplaySprite.Center += relativePosition;
		}

		private Vector2D lastScalePosition = Vector2D.Unused;

		public void ScaleImage(Vector2D mousePosition)
		{
			var relativePosition = mousePosition - lastScalePosition;
			lastScalePosition = mousePosition;
			currentDisplaySprite.Size =
				new Size(
					currentDisplaySprite.Size.Width + (currentDisplaySprite.Size.Width * relativePosition.Y),
					currentDisplaySprite.Size.Height + (currentDisplaySprite.Size.Height * relativePosition.Y));
		}
	}
}