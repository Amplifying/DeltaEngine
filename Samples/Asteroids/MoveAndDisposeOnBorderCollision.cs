using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Asteroids
{
	internal class MoveAndDisposeOnBorderCollision : EntityHandler
	{
		public MoveAndDisposeOnBorderCollision(ScreenSpace screen)
		{
			this.screen = screen;
			this.entitySystem = entitySystem;
		}

		private readonly ScreenSpace screen;
		private readonly EntitySystem entitySystem;

		public override void Handle(List<Entity> entities)
		{
			foreach (Projectile projectile in entities.OfType<Projectile>())
			{
				projectile.DrawArea =
					new Rectangle(
						projectile.DrawArea.TopLeft +
							projectile.Get<SimplePhysics.Data>().Velocity * Time.Current.Delta,
						projectile.DrawArea.Size);

				if (projectile.DrawArea.Right <= screen.Viewport.Left ||
					projectile.DrawArea.Left >= screen.Viewport.Right ||
					projectile.DrawArea.Bottom <= screen.Viewport.Top ||
					projectile.DrawArea.Top >= screen.Viewport.Bottom)
					projectile.IsActive = false;
			}
		}

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}