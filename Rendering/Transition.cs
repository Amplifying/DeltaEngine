using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Transitions the position, size, color and/or rotation of an Entity2D
	/// </summary>
	public class Transition : EntityHandler
	{
		public override void Handle(List<Entity> entities)
		{
			foreach (Entity2D entity in entities.OfType<Entity2D>())
				TransitionEntity(entity);
		}

		private void TransitionEntity(Entity2D entity)
		{
			var duration = entity.GetOrCreate<Duration>();
			if (duration.Elapsed >= duration.Value)
				return;

			var percentage = duration.Elapsed / duration.Value;
			UpdateEntityColor(entity, percentage);
			UpdateEntityOutlineColor(entity, percentage);
			UpdateEntityPosition(entity, percentage);
			UpdateEntitySize(entity, percentage);
			UpdateEntityRotation(entity, percentage);
			CheckForEndOfTransition(entity, duration);
		}

		private static void UpdateEntityColor(Entity2D entity, float percentage)
		{
			if (!entity.Contains<Color>())
				return;

			var transitionColor = entity.Get<Color>();
			entity.Color = Datatypes.Color.Lerp(transitionColor.Start, transitionColor.End, percentage);
		}

		private static void UpdateEntityOutlineColor(Entity2D entity, float percentage)
		{
			if (!entity.Contains<OutlineColor>() || !entity.Contains<Shapes.OutlineColor>())
				return;

			var transitionOutlineColor = entity.Get<OutlineColor>();
			entity.Set(
				new Shapes.OutlineColor(Datatypes.Color.Lerp(transitionOutlineColor.Start,
					transitionOutlineColor.End, percentage)));
		}

		protected virtual void UpdateEntityPosition(Entity2D entity, float percentage)
		{
			if (!entity.Contains<Position>())
				return;

			var transitionPosition = entity.Get<Position>();
			entity.Center = Point.Lerp(transitionPosition.Start, transitionPosition.End, percentage);
		}

		private static void UpdateEntitySize(Entity2D entity, float percentage)
		{
			if (!entity.Contains<Size>())
				return;

			var transitionSize = entity.Get<Size>();
			var center = entity.Center;
			entity.Size = Datatypes.Size.Lerp(transitionSize.Start, transitionSize.End, percentage);
			if (!entity.Contains<Position>())
				entity.TopLeft = center - entity.Size / 2.0f;
		}

		private static void UpdateEntityRotation(Entity2D entity, float percentage)
		{
			if (!entity.Contains<Rotation>())
				return;

			var transitionRotation = entity.Get<Rotation>();
			entity.Rotation = MathExtensions.Lerp(transitionRotation.Start, transitionRotation.End,
				percentage);
		}

		private void CheckForEndOfTransition(Entity entity, Duration duration)
		{
			duration.Elapsed += Time.Current.Delta;
			if (duration.Elapsed < duration.Value)
				return;

			RemoveTransitionComponents(entity);
			entity.MessageAllListeners(new TransitionEnded());
			EndTransition(entity);
		}

		public class TransitionEnded {}

		protected virtual void EndTransition(Entity entity) {}

		private static void RemoveTransitionComponents(Entity entity)
		{
			if (entity.Contains<Color>())
				entity.Remove<Color>();

			if (entity.Contains<OutlineColor>())
				entity.Remove<OutlineColor>();

			if (entity.Contains<Position>())
				entity.Remove<Position>();

			if (entity.Contains<Size>())
				entity.Remove<Size>();

			if (entity.Contains<Rotation>())
				entity.Remove<Rotation>();
		}

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}

		/// <summary>
		/// Duration, Color, FadingColor, OutlineColor, Position, Rotation, Size are all Components
		/// that can be added to an Entity undergoing a Transition
		/// </summary>
		public class Duration
		{
			public Duration()
				: this(1.0f) {}

			public Duration(float duration)
			{
				Value = duration;
			}

			public float Value { get; set; }
			public float Elapsed { get; set; }
		}

		public class Color
		{
			public Color(Datatypes.Color startColor, Datatypes.Color endColor)
			{
				Start = startColor;
				End = endColor;
			}

			public Datatypes.Color Start { get; set; }
			public Datatypes.Color End { get; set; }
		}

		public class FadingColor : Color
		{
			public FadingColor()
				: this(Datatypes.Color.White) {}

			public FadingColor(Datatypes.Color startColor)
				: base(startColor, Datatypes.Color.Transparent(startColor)) {}
		}

		public class OutlineColor
		{
			public OutlineColor(Datatypes.Color startColor, Datatypes.Color endColor)
			{
				Start = startColor;
				End = endColor;
			}

			public Datatypes.Color Start { get; set; }
			public Datatypes.Color End { get; set; }
		}

		public class Position
		{
			public Position(Point startPosition, Point endPosition)
			{
				Start = startPosition;
				End = endPosition;
			}

			public Point Start { get; set; }
			public Point End { get; set; }
		}

		public class Rotation
		{
			public Rotation(float startRotation, float endRotation)
			{
				Start = startRotation;
				End = endRotation;
			}

			public float Start { get; set; }
			public float End { get; set; }
		}

		public class Size
		{
			public Size(Datatypes.Size startSize, Datatypes.Size endSize)
			{
				Start = startSize;
				End = endSize;
			}

			public Datatypes.Size Start { get; set; }
			public Datatypes.Size End { get; set; }
		}
	}
}