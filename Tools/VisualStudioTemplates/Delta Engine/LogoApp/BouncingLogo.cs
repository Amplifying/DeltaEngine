using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace $safeprojectname$
{
	/// <summary>
	/// Extends a sprite and displays a colored Delta Engine logo bouncing around with random values.
	/// </summary>
	public class BouncingLogo : Sprite
	{
		public BouncingLogo(Content content, Randomizer random)
			: base(content.Load<Image>("DeltaEngineLogo"), Rectangle.One, Color.GetRandomColor())
		{
			DrawArea = Rectangle.FromCenter(Point.Half, new Size(random.Get(0.1f, 0.2f)));
			velocity = new Point(random.Get(-0.4f, +0.4f), random.Get(-0.4f, +0.4f));
			Rotation = random.Get(0, 360);
			rotationSpeed = random.Get(-50, 50);
		}

		protected Point velocity;
		protected float rotationSpeed;

		protected override void Render(Renderer renderer, Time time)
		{
			Rotation += rotationSpeed * time.CurrentDelta;
			DrawArea.Center += velocity * time.CurrentDelta;
			velocity.ReflectIfHittingBorder(DrawArea, renderer.Screen.Viewport);
			base.Render(renderer, time);
		}
	}
}