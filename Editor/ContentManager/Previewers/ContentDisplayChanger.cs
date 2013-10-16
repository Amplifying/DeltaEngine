using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Particles;

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

		public static Vector2D lastPanPosition = Vector2D.Unused;

		public static void MoveParticle(Vector2D mousePosition, Entity2D entity)
		{
			var distance = CalculateDistanceFromOldMousePosition(mousePosition, entity);
			entity.Center += distance;
		}

		public static Vector2D CalculateDistanceFromOldMousePosition(Vector2D mousePosition,
			Entity entity)
		{
			if (entity == null)
				return new Vector2D(0, 0);//ncrunch: no coverage 
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

		public static Vector2D lastScalePosition = Vector2D.Unused;

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

		public static Vector2D lastRotatePosition;
		public static Vector2D lastZoomPosition = Vector2D.Unused;
	}
}