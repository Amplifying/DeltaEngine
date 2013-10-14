using DeltaEngine.Content;
using DeltaEngine.GameLogic;
using DeltaEngine.Rendering3D.Shapes3D;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	internal class LevelPreview : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			var level = ContentLoader.Load<Level>(contentName);
			new LevelRenderer(level);
		}
	}
}