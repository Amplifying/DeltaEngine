using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class SpriteSheetPreviewer : ContentPreview
	{
		public override void Preview(string contentName)
		{
			currentDisplayAnimation = new Sprite(new Material("Position2DUV", contentName),
				new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
			try
			{
				ContentLoader.Load<Image>(currentDisplayAnimation.Material.SpriteSheet.Image.Name);
			}
			catch (Exception)
			{
				UnableToLoadContentMessage.SendUnableToLoadContentMessage(contentName);
				currentDisplayAnimation.IsActive = false;
			}
		}

		public Sprite currentDisplayAnimation;
	}
}