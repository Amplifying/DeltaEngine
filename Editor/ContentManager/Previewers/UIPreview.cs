using DeltaEngine.Content;
using DeltaEngine.Scenes;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	class UIPreview : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			ContentLoader.Load<Scene>(contentName);
		}
	}
}
