using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;

namespace DeltaEngine.Physics2D
{
	public class Physics2D : EntityHandler
	{
		private readonly ScreenSpace screen;

		public Physics2D(ScreenSpace screen)
		{
			this.screen = screen;
		}

		public void Handle(List<Entity> entities)
		{
			foreach (var entity in entities.OfType<Entity2D>())
			{
				var physicsBody = entity.Get<PhysicsBody>();
				if (physicsBody != null)
					UpdatePositionAndRotation(entity, physicsBody);
			}
		}

		private void UpdatePositionAndRotation(Entity2D entity, PhysicsBody physicsBody)
		{
			var drawArea = entity.DrawArea;
			entity.DrawArea = Rectangle.FromCenter(screen.FromPixelSpace(physicsBody.Position),
				drawArea.Size);
			entity.Rotation = physicsBody.Rotation;
		}

		public EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}