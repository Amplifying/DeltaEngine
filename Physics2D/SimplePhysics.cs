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
		public class Rotate : EntityHandler
		{
			public override void Handle(Entity entity)
			{
				entity.Set(entity.Get<float>() + entity.Get<Data>().RotationSpeed * Time.Current.Delta);
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}

		/// <summary>
		/// Bounces an Entity2D off the edges of the screen
		/// </summary>
		public class BounceOffScreenEdges : EntityHandler
		{
			public BounceOffScreenEdges(ScreenSpace screen)
			{
				this.screen = screen;
			}

			private readonly ScreenSpace screen;

			public override void Handle(Entity entity)
			{
				var physics = entity.Get<Data>();
				var drawArea = entity.Get<Rectangle>();
				drawArea = new Rectangle(drawArea.TopLeft + physics.Velocity * Time.Current.Delta,
					drawArea.Size);
				entity.Set(drawArea);
				Point velocity = physics.Velocity;
				velocity.ReflectIfHittingBorder(drawArea, screen.Viewport);
				physics.Velocity = velocity;
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}

		/// <summary>
		/// Causes an Entity2D to move and fall under gravity.
		/// When the duration is complete it removes the Entity from the Entity System
		/// </summary>
		public class Fall : EntityHandler
		{
			public override void Handle(Entity entity)
			{
				var entity2D = entity as Entity2D;
				var physics = entity2D.Get<Data>();
				physics.Velocity += physics.Gravity * Time.Current.Delta;
				entity2D.Center += physics.Velocity * Time.Current.Delta;
				entity2D.Rotation += physics.RotationSpeed * Time.Current.Delta;
				physics.Elapsed += Time.Current.Delta;
				if (physics.Duration > 0.0f && physics.Elapsed >= physics.Duration)
					entity.IsActive = false;
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}
	}
}