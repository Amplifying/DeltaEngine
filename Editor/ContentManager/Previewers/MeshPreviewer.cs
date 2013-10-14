using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering3D;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public sealed class MeshPreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			var modelData = ContentLoader.Load<ModelData>(contentName);
			new Model(modelData, new Vector3D(0, 0, 0));
			ContentDisplayChanger.Set3DMovementCommand();
			ContentDisplayChanger.Set3DRotationCommand();
			ContentDisplayChanger.Set3DZoomingCommand();
		}
	}
}