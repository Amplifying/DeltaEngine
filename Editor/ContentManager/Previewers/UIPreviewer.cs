using DeltaEngine.Content;
using DeltaEngine.Scenes;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class UIPreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			ContentLoader.Load<Scene>(contentName);
		}
	}
}
