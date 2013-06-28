using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Physics2D
{
	public class Physics2D : Behavior2D
	{
		private readonly ScreenSpace screen;

		public Physics2D(ScreenSpace screen)
		{
			this.screen = screen;
		}
		public override void Handle(Entity2D entity)
		{
			var physicsBody = entity.Get<PhysicsBody>();
			var size = entity.Get<Rectangle>().Size;
			entity.Set(Rectangle.FromCenter(screen.FromPixelSpace(physicsBody.Position), size));
			entity.Set(physicsBody.Rotation);
		}

		public override Priority Priority
		{
			get { return Priority.First; }
		}
	}
}