using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class MaterialPreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			var material = ContentLoader.Load<Material>(contentName);
			var imageSize = material.DiffuseMap.PixelSize;
			var aspectRatio = imageSize.Height / imageSize.Width;
			currentDisplaySprite = new Sprite(material,
				Rectangle.FromCenter(new Vector2D(0.5f, 0.5f), new Size(0.5f, 0.5f * aspectRatio)));
			SetImageCommands();
		}

		public Sprite currentDisplaySprite;

		private void SetImageCommands()
		{
			ContentDisplayChanger.SetEntity2DMoveCommand(currentDisplaySprite);
			ContentDisplayChanger.SetEntity2DScaleCommand(currentDisplaySprite);
		}
	}
}