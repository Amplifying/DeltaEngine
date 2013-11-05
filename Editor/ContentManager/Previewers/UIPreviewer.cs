using DeltaEngine.Content;
using DeltaEngine.Scenes;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class UIPreviewer : ContentPreview
	{
		public override void Preview(string contentName)
		{
			var scene = ContentLoader.Load<Scene>(contentName);
			foreach (var control in scene.Controls)
				control.IsActive = true;
		}
	}
}
