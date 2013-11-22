using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public sealed class ImageAnimationPreviewer : ContentPreview
	{
		public override void Preview(string contentName)
		{
			currentDisplayAnimation = new Sprite(new Material(Shader.Position2DUV, contentName),
				new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
			
				try
				{
					foreach (var frame in currentDisplayAnimation.Material.Animation.Frames)
					{
						ContentLoader.Load<Image>(frame.Name);
					}
				}
				//ncrunch: no coverage start 
				catch (Exception)
				{
					UnableToLoadContentMessage.SendUnableToLoadContentMessage(contentName);
					currentDisplayAnimation.IsActive = false;
				}	//ncrunch: no coverage end
		}

		public Sprite currentDisplayAnimation;
	}
}