using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Physics2D
{
	/// <summary>
	/// Groups the various physics effects that can be applied to Entities
	/// </summary>
	public class SimplePhysics
	{
		/// <summary>
		/// Holds physics related data
		/// </summary>
		public class Data
		{
			public float Elapsed { get; set; }
			public float Duration { get; set; }
			public float RotationSpeed { get; set; }
			public Point Velocity { get; set; }
			public Point Gravity { get; set; }
		}

		/// <summary>
		/// Rotates an Entity2D every frame
		/// </summary>
		public class Rotate : Behavior2D
		{
			public override void Handle(Entity2D entity)
			{
				entity.Set(entity.Get<float>() + entity.Get<Data>().RotationSpeed * Time.Current.Delta);
			}

			public override Priority Priority
			{
				get { return Priority.First; }
			}
		}

		/// <summary>
		/// Bounces an Entity2D off the edges of the screen
		/// </summary>
		public class BounceOffScreenEdges : Behavior2D
		{
			public BounceOffScreenEdges(ScreenSpace screen)
			{
				this.screen = screen;
			}

			private readonly ScreenSpace screen;

			public override void Handle(Entity2D entity)
			{
				var physics = entity.Get<Data>();
				entity.DrawArea = new Rectangle(entity.TopLeft + physics.Velocity * Time.Current.Delta,
					entity.Size);
				Point velocity = physics.Velocity;
				velocity.ReflectIfHittingBorder(entity.DrawArea, screen.Viewport);
				physics.Velocity = velocity;
			}

			public override Priority Priority
			{
				get { return Priority.First; }
			}
		}

		/// <summary>
		/// Causes an Entity2D to move and fall under gravity.
		/// When the duration is complete it removes the Entity from the Entity System
		/// </summary>
		public class Fall : Behavior2D
		{
			public override void Handle(Entity2D entity)
			{
				var physics = entity.Get<Data>();
				physics.Velocity += physics.Gravity * Time.Current.Delta;
				entity.Center += physics.Velocity * Time.Current.Delta;
				entity.Rotation += physics.RotationSpeed * Time.Current.Delta;
				physics.Elapsed += Time.Current.Delta;
				if (physics.Duration > 0.0f && physics.Elapsed >= physics.Duration)
					entity.IsActive = false;
			}

			public override Priority Priority
			{
				get { return Priority.First; }
			}
		}
	}
}