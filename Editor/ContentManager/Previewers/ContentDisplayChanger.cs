using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Particles;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class ContentDisplayChanger
	{
		public static void SetEntity2DMoveCommand(Entity2D entity2D)
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(position => MoveParticle(position, entity2D)).Add(
				new MousePositionTrigger(MouseButton.Left, State.Pressed));
		}

		private static Vector2D lastPanPosition = Vector2D.Unused;

		public static void MoveParticle(Vector2D mousePosition, Entity2D entity)
		{
			var distance = CalculateDistanceFromOldMousePosition(mousePosition, entity);
			entity.Center += distance;
		}

		private static Vector2D CalculateDistanceFromOldMousePosition(Vector2D mousePosition,
			Entity entity)
		{
			if (entity == null)
				return new Vector2D(0, 0);
			var relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			return relativePosition;
		}

		public static void SetEntity2DScaleCommand(Entity2D entity2D)
		{
			new Command(position => lastScalePosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
			new Command(position => ScaleImage(position, entity2D)).Add(
				new MousePositionTrigger(MouseButton.Middle, State.Pressed));
		}

		private static Vector2D lastScalePosition = Vector2D.Unused;

		public static void ScaleImage(Vector2D mousePosition, Entity2D entity)
		{
			var relativePosition = mousePosition - lastScalePosition;
			lastScalePosition = mousePosition;
			entity.Size = new Size(entity.Size.Width + (entity.Size.Width * relativePosition.Y),
				entity.Size.Height + (entity.Size.Height * relativePosition.Y));
		}

		public static void SetParticleDMoveCommand(ParticleEmitter entity3D)
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(position => MoveParticle(position, entity3D)).Add(
				new MousePositionTrigger(MouseButton.Left, State.Pressed));
		}

		public static void MoveParticle(Vector2D mousePosition, ParticleEmitter entity)
		{
			var distance = CalculateDistanceFromOldMousePosition(mousePosition, entity);
			entity.Position += distance;
		}

		public static void Set3DMovementCommand()
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(position => MoveCamera(position)).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
		}

		private static void MoveCamera(Vector2D mousePosition)
		{
			if (camera3D == null)
				SetCamera();
			Vector2D relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			if (relativePosition == Vector2D.Zero)
				return;
			var cameraPan = new Vector3D(-relativePosition.X, relativePosition.Y, 0.0f);
			camera3D.Position += cameraPan;
			camera3D.Target += cameraPan;
		}

		private static LookAtCamera camera3D;

		private static void SetCamera()
		{
			camera3D = Camera.Use<LookAtCamera>();
			camera3D.Position = 10 * Vector3D.One;
			camera3D.Target = Vector3D.Zero;
		}

		public static void Set3DRotationCommand()
		{
			new Command(position => lastRotatePosition = position).Add(
				new MouseButtonTrigger(MouseButton.Right));
			new Command(RotateCamera).Add(new MousePositionTrigger(MouseButton.Right, State.Pressed));
		}

		private static void RotateCamera(Vector2D mousePosition)
		{
			if (camera3D == null)
				SetCamera();
			Vector2D relativePosition = mousePosition - lastRotatePosition;
			lastRotatePosition = mousePosition;
			if (relativePosition == Vector2D.Zero)
				return;
			camera3D.YawPitchRoll += new Vector3D(-relativePosition.Y * 300.0f, relativePosition.X * 300.0f,
				0.0f);
		}

		private static Vector2D lastRotatePosition;

		public static void Set3DZoomingCommand()
		{
			
			new Command(position => lastZoomPosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
			new Command(ZoomCamera).Add(new MousePositionTrigger(MouseButton.Middle, State.Pressed));
			
		}

		private static void ZoomCamera(Vector2D mousePosition)
		{
			if (camera3D == null)
				SetCamera();
			Vector2D relativePosition = mousePosition - lastZoomPosition;
			lastZoomPosition = mousePosition;
			if (relativePosition == Vector2D.Zero)
				return;
			camera3D.Zoom(relativePosition.Y);
		}

		private static Vector2D lastZoomPosition = Vector2D.Unused;
	}
}