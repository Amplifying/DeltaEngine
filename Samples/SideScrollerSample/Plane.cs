using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.Sprites;

namespace SideScrollerSample
{
	public abstract class Plane : Sprite
	{
		protected Plane(Image texture, Point initialPosition, Color color)
			: base(texture, Rectangle.FromCenter(initialPosition, new Size(0.2f, 0.1f)), color)
		{
			Add<HitPointsHandler>();
		}

		internal float verticalDecelerationFactor, verticalAccelerationFactor;

		protected const float MaximumSpeed = 2;
		public int Hitpoints { get; protected set; }
		internal bool defeated;

		protected class HitPointsHandler : EntityHandler
		{
			public HitPointsHandler()
			{
				Filter = entity => entity is Plane;
			}

			public override void Handle(Entity entity)
			{
				var plane = entity as Plane;
					if (plane.Hitpoints <= 0)
						plane.defeated = true;
			}
		}

		public void AccelerateVertically(float magnitude)
		{
			Get<Velocity2D>().Accelerate(new Point(0, verticalAccelerationFactor * magnitude));
			verticalDecelerationFactor = 0.8f;
		}

		public void StopVertically()
		{
			verticalDecelerationFactor = 4.0f;
		}

		public float YPosition
		{
			get { return Get<Rectangle>().Center.Y; }
		}
	}
}