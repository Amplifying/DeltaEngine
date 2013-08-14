using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Transitions the position, size, color and/or rotation of an Entity2D
	/// </summary>
	public class Transition : UpdateBehavior
	{
		public Transition() : base(Priority.Low) { }

		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (var entity in entities)
			{
				var duration = entity.Get<Duration>();
				if (duration.Elapsed >= duration.Value)
					continue;

				var percentage = duration.Elapsed / duration.Value;
				UpdateEntityColor((Entity2D)entity, percentage);
				UpdateEntityTransition((Entity2D)entity, percentage);
				UpdateEntityOutlineColor((Entity2D)entity, percentage);
				UpdateEntityPosition((Entity2D)entity, percentage);
				UpdateEntitySize((Entity2D)entity, percentage);
				UpdateEntityRotation((Entity2D)entity, percentage);
				CheckForEndOfTransition(entity, duration);
			}		
		}

		private static void UpdateEntityColor(Entity2D entity, float percentage)
		{
			if (!entity.Contains<ColorRange>())
				return;

			var transitionColor = entity.Get<ColorRange>();
			entity.Color = transitionColor.Start.Lerp(transitionColor.End, percentage);
		}

		private static void UpdateEntityTransition(Entity2D entity, float percentage)
		{
			if (!entity.Contains<FadingColor>())
				return;

			var fadingColor = entity.Get<FadingColor>();
			entity.Color = fadingColor.Start.Lerp(fadingColor.End, percentage);
		}


		private static void UpdateEntityOutlineColor(Entity2D entity, float percentage)
		{
			if (!entity.Contains<OutlineColor>() || !entity.Contains<Shapes.OutlineColor>())
				return;

			var transitionOutlineColor = entity.Get<OutlineColor>();
			entity.Set(
				new Shapes.OutlineColor(transitionOutlineColor.Start.Lerp(
					transitionOutlineColor.End, percentage)));
		}

		protected virtual void UpdateEntityPosition(Entity2D entity, float percentage)
		{
			if (!entity.Contains<Position>())
				return;

			var transitionPosition = entity.Get<Position>();
			entity.Center = transitionPosition.Start.Lerp(transitionPosition.End, percentage);
		}

		private static void UpdateEntitySize(Entity2D entity, float percentage)
		{
			if (!entity.Contains<SizeRange>())
				return;

			var transitionSize = entity.Get<SizeRange>();
			var center = entity.Center;
			entity.Size = transitionSize.Start.Lerp(transitionSize.End, percentage);
			if (!entity.Contains<Position>())
				entity.TopLeft = center - entity.Size / 2.0f;
		}

		private static void UpdateEntityRotation(Entity2D entity, float percentage)
		{
			if (!entity.Contains<Rotation>())
				return;

			var transitionRotation = entity.Get<Rotation>();
			entity.Rotation = transitionRotation.Start.Lerp(transitionRotation.End, percentage);
		}

		private void CheckForEndOfTransition(Entity entity, Duration duration)
		{
			duration.Elapsed += Time.Delta;
			if (duration.Elapsed < duration.Value)
			{
				entity.Set(duration);
				return;
			}
			
			RemoveTransitionComponents(entity);
			EndTransition(entity);
		}

		protected virtual void EndTransition(Entity entity) {}

		private static void RemoveTransitionComponents(Entity entity)
		{
			if (entity.Contains<ColorRange>())
				entity.Remove<ColorRange>();

			if (entity.Contains<OutlineColor>())
				entity.Remove<OutlineColor>();

			if (entity.Contains<Position>())
				entity.Remove<Position>();

			if (entity.Contains<SizeRange>())
				entity.Remove<SizeRange>();

			if (entity.Contains<Rotation>())
				entity.Remove<Rotation>();
		}

		/// <summary>
		/// Duration, Color, FadingColor, OutlineColor, Position, Rotation, Size are all Components
		/// that can be added to an Entity undergoing a Transition
		/// </summary>
		public struct Duration
		{
			public Duration(float duration)
				: this()
			{
				Value = duration;
			}

			public float Value { get; private set; }
			public float Elapsed { get; internal set; }
		}

		public struct ColorRange
		{
			public ColorRange(Color startColor, Color endColor)
				: this()
			{
				Start = startColor;
				End = endColor;
			}

			public Color Start { get; private set; }
			public Color End { get; private set; }
		}

		public struct FadingColor
		{
			public FadingColor(Color startColor)
				: this()
			{
				Start = startColor;
				End = Color.Transparent(startColor);
			}

			public Color Start { get; private set; }
			public Color End { get; private set; }
		}

		public struct OutlineColor
		{
			public OutlineColor(Color startColor, Color endColor)
				: this()
			{
				Start = startColor;
				End = endColor;
			}

			public Color Start { get; private set; }
			public Color End { get; private set; }
		}

		public struct Position
		{
			public Position(Point startPosition, Point endPosition)
				: this()
			{
				Start = startPosition;
				End = endPosition;
			}

			public Point Start { get; private set; }
			public Point End { get; private set; }
		}

		public struct Rotation
		{
			public Rotation(float startRotation, float endRotation)
				: this()
			{
				Start = startRotation;
				End = endRotation;
			}

			public float Start { get; private set; }
			public float End { get; private set; }
		}

		public struct SizeRange
		{
			public SizeRange(Size startSize, Size endSize)
				: this()
			{
				Start = startSize;
				End = endSize;
			}

			public Size Start { get; private set; }
			public Size End { get; private set; }
		}
	}
}