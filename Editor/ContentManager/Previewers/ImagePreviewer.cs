using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

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
			currentDisplaySprite = new Sprite(new Material(Shader.Position2DUV, contentName),
				Rectangle.FromCenter(new Vector2D(0.5f, 0.5f), new Size(0.5f, 0.5f * aspectRatio)));
			ContentDisplayChanger.SetEntity2DMoveCommand(currentDisplaySprite);
			ContentDisplayChanger.SetEntity2DScaleCommand(currentDisplaySprite);
		}

		public Sprite currentDisplaySprite;
	}
}