using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public sealed class ImageAnimationPreviewer : ContentPreview
	{
		public override void Preview(string contentName)
		{
			currentDisplayAnimation = new Sprite(new Material(Shader.Position2DUV, contentName),
				new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
		}

		public Sprite currentDisplayAnimation;
	}
}