using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Physics2D
{
	public class Physics2D : EntityHandler
	{
		private readonly ScreenSpace screen;

		public Physics2D(ScreenSpace screen)
		{
			this.screen = screen;
		}

		public override void Handle(Entity entity)
		{
			var physicsBody = entity.Get<PhysicsBody>();
			var size = entity.Get<Rectangle>().Size;
			entity.Set(Rectangle.FromCenter(screen.FromPixelSpace(physicsBody.Position), size));
			entity.Set(physicsBody.Rotation);
		}

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}