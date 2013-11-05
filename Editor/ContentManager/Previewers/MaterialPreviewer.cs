using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class MaterialPreviewer : ContentPreview
	{
		public override void Preview(string contentName)
		{
			var material = ContentLoader.Load<Material>(contentName);
			var imageSize = material.DiffuseMap.PixelSize;
			var aspectRatio = imageSize.Height / imageSize.Width;
			currentDisplaySprite = new Sprite(material,
				Rectangle.FromCenter(new Vector2D(0.5f, 0.5f), new Size(0.5f, 0.5f * aspectRatio)));
		}

		public Sprite currentDisplaySprite;
	}
}