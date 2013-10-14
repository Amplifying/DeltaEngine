using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public sealed class ImageAnimationPreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			currentDisplayAnimation = new Sprite(new Material(Shader.Position2DUV, contentName),
				new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
			ContentDisplayChanger.SetEntity2DMoveCommand(currentDisplayAnimation);
			ContentDisplayChanger.SetEntity2DScaleCommand(currentDisplayAnimation);
		}

		public Sprite currentDisplayAnimation;
	}
}