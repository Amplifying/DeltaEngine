using DeltaEngine.Content;
using DeltaEngine.Multimedia;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class VideoPreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			var video = ContentLoader.Load<Video>(contentName);
			video.Play();
		}
	}
}