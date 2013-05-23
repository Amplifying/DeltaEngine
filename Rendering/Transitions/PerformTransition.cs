using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering.Shapes;

namespace DeltaEngine.Rendering.Transitions
{
	/// <summary>
	/// Transitions the position, size and/or color of an Entity2D
	/// </summary>
	public class PerformTransition : EntityHandler
	{
		public PerformTransition(EntitySystem entitySystem)
		{
			this.entitySystem = entitySystem;
		}

		private readonly EntitySystem entitySystem;

		public void Handle(List<Entity> entities)
		{
			foreach (Entity2D entity in entities.OfType<Entity2D>().Where(e => e.Contains<Transition>()))
				FadeEntity(entity);
		}

		private void FadeEntity(Entity2D entity)
		{
			var transition = entity.Get<Transition>();
			if (transition.Elapsed >= transition.Duration)
				return;

			var percentage = transition.Elapsed / transition.Duration;
			UpdateEntityColor(entity, percentage);
			UpdateEntityOutlineColor(entity, percentage);
			UpdateEntityPosition(entity, percentage);
			UpdateEntitySize(entity, percentage);
			UpdateEntityRotation(entity, percentage);
			CheckForEndOfTransition(entity, transition);
		}

		private static void UpdateEntityColor(Entity2D entity, float percentage)
		{
			if (!entity.Contains<TransitionColor>())
				return;

			var transitionColor = entity.Get<TransitionColor>();
			entity.Color = Color.Lerp(transitionColor.Start, transitionColor.End, percentage);
		}

		private static void UpdateEntityOutlineColor(Entity2D entity, float percentage)
		{
			if (!entity.Contains<OutlineColor>() || !entity.Contains<TransitionOutlineColor>())
				return;

			var transitionOutlineColor = entity.Get<TransitionOutlineColor>();
			entity.Set(
				new OutlineColor(Color.Lerp(transitionOutlineColor.Start, transitionOutlineColor.End,
					percentage)));
		}

		private static void UpdateEntityPosition(Entity2D entity, float percentage)
		{
			if (!entity.Contains<TransitionPosition>())
				return;

			var transitionPosition = entity.Get<TransitionPosition>();
			entity.TopLeft = Point.Lerp(transitionPosition.Start, transitionPosition.End, percentage);
		}

		private static void UpdateEntitySize(Entity2D entity, float percentage)
		{
			if (!entity.Contains<TransitionSize>())
				return;

			var transitionSize = entity.Get<TransitionSize>();
			var center = entity.Center;
			entity.Size = Size.Lerp(transitionSize.Start, transitionSize.End, percentage);
			if (!entity.Contains<TransitionPosition>())
				entity.TopLeft = center - entity.Size / 2.0f;
		}

		private static void UpdateEntityRotation(Entity2D entity, float percentage)
		{
			if (!entity.Contains<TransitionRotation>())
				return;

			var transitionRotation = entity.Get<TransitionRotation>();
			entity.Rotation = MathExtensions.Lerp(transitionRotation.Start, transitionRotation.End,
				percentage);
		}

		private void CheckForEndOfTransition(Entity entity, Transition transition)
		{
			transition.Elapsed += Time.Current.Delta;
			if (transition.Elapsed < transition.Duration)
				return;

			RemoveTransitionComponents(entity);
			entity.MessageAllListeners(new TransitionEnded());
			if (transition.IsEntityToBeRemovedWhenFinished)
				entitySystem.Remove(entity);
		}

		private static void RemoveTransitionComponents(Entity entity)
		{
			if (entity.Contains<TransitionColor>())
				entity.Remove<TransitionColor>();

			if (entity.Contains<TransitionOutlineColor>())
				entity.Remove<TransitionOutlineColor>();

			if (entity.Contains<TransitionPosition>())
				entity.Remove<TransitionPosition>();

			if (entity.Contains<TransitionSize>())
				entity.Remove<TransitionSize>();

			if (entity.Contains<TransitionRotation>())
				entity.Remove<TransitionRotation>();
		}

		public class TransitionEnded {}

		public EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}