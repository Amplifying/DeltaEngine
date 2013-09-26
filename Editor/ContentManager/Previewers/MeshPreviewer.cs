using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Models;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public sealed class MeshPreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			SetCamera();
			var modeldata = ContentLoader.Load<ModelData>(contentName);
			new Model(modeldata, new Vector3D(0, 0, 0));
			Set3DModelCommands();
		}

		private void SetCamera()
		{
			camera3D = Camera.Use<LookAtCamera>();
			camera3D.Position = 10 * Vector3D.One;
			camera3D.Target = Vector3D.Zero;
		}

		private Camera camera3D;

		private void Set3DModelCommands()
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(MoveCamera).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
			new Command(position => lastZoomPosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
			//new Command(ZoomCamera).Add(new MousePositionTrigger(MouseButton.Middle, State.Pressed));
			//new Command(position => lastRotatePosition = position).Add(
			//	new MouseButtonTrigger(MouseButton.Right));
			//new Command(RotateCamera).Add(new MousePositionTrigger(MouseButton.Right, State.Pressed));
		}

		private Vector2D lastPanPosition = Vector2D.Unused;

		private void MoveCamera(Vector2D mousePosition)
		{
			if(camera3D == null)
				SetCamera();
			Vector2D relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			if (relativePosition == Vector2D.Zero)
				return;
			var cameraPan = new Vector3D(-relativePosition.X, relativePosition.Y, 0.0f);
			camera3D.Position += cameraPan;
			camera3D.Target += cameraPan;
		}

		private Vector2D lastZoomPosition = Vector2D.Unused;

		//private void ZoomCamera(Vector2D mousePosition)
		//{
		//	Vector2D relativePosition = mousePosition - lastZoomPosition;
		//	lastZoomPosition = mousePosition;
		//	if (relativePosition == Vector2D.Zero)
		//		return;
		//	Camera.Current.Zoom(relativePosition.Y);
		//}

		//private Vector2D lastRotatePosition;

		//private void RotateCamera(Vector2D mousePosition)
		//{
		//	Vector2D relativePosition = mousePosition - lastRotatePosition;
		//	lastRotatePosition = mousePosition;
		//	if (relativePosition == Vector2D.Zero)
		//		return;
		//	Camera.Current. += new Vector3D(-relativePosition.Y * 300.0f, relativePosition.X * 300.0f,
		//		0.0f);
		//}
	}
}