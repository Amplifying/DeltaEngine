using DeltaEngine.Content;
using DeltaEngine.Multimedia;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class VideoPreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			video = ContentLoader.Load<Video>(contentName);
			video.Play();
		}

		public Video video;
	}
}