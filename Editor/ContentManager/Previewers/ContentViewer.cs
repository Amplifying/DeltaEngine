using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class ContentViewer
	{
		public ContentViewer()
		{
			viewers.Add(ContentType.Image, new ImagePreviewer());
			viewers.Add(ContentType.ImageAnimation, new ImageAnimationPreviewer());
			viewers.Add(ContentType.SpriteSheetAnimation, new SpriteSheetPreviewer());
			viewers.Add(ContentType.Material, new MaterialPreviewer());
			viewers.Add(ContentType.ParticleEmitter, new ParticlePreviewer());
			viewers.Add(ContentType.Font, new FontPreviewer());
			viewers.Add(ContentType.Sound, new SoundPreviewer());
			viewers.Add(ContentType.Music, new MusicPreviewer());
			viewers.Add(ContentType.Xml, new XmlPreviewer());
			viewers.Add(ContentType.Scene, new UIPreviewer());
		}

		private readonly Dictionary<ContentType, ContentPreview> viewers =
			new Dictionary<ContentType, ContentPreview>();

		public void View(string contentName, ContentType type)
		{
			if (type == ContentType.Shader)
				return;
			viewers[type].PreviewContent(contentName);
		}
	}
}