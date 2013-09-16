using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content;

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
			//viewers.Add(ContentType.Mesh, new MeshPreviewer((DefaultCamera)camera));
			viewers.Add(ContentType.Particle2DEmitter, new ParticlePreviewer());
			viewers.Add(ContentType.Font, new FontPreviewer());
			viewers.Add(ContentType.Sound, new SoundPreviewer());
			viewers.Add(ContentType.Music, new MusicPreviewer());
			viewers.Add(ContentType.Video, new VideoPreviewer());
			viewers.Add(ContentType.Xml, new XmlPreviewer());
			viewers.Add(ContentType.Scene, new UIPreview());
		}

		private readonly Dictionary<ContentType, ContentPreview> viewers =
			new Dictionary<ContentType, ContentPreview>();

		public void Viewer(string contentName, string projectName, ContentType type)
		{
			if (type == ContentType.Shader)
				return;
			if (type == ContentType.Xml)
				viewers[type].PreviewContent(Path.Combine(contentFolder, projectName, contentName, ".xml"));
			viewers[type].PreviewContent(contentName);
		}

		private const string contentFolder = "Content";
	}
}