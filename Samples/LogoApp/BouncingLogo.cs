using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;

namespace LogoApp
{
	/// <summary>
	/// Colored Delta Engine logo which spins and bounces around the screen
	/// </summary>
	public class BouncingLogo : PhysicsSprite
	{
		public BouncingLogo(ContentLoader content)
			: base(
				content.Load<Image>("DeltaEngineLogo"), new Rectangle(0.4f, 0.4f, 0.2f, 0.2f),
				Color.GetRandomColor())
		{
			Gravity = Point.Zero;
			Rotation = Randomizer.Current.Get(0, 360);
			RotationSpeed = Randomizer.Current.Get(-50, 50);
			Velocity = new Point(Randomizer.Current.Get(-0.4f, +0.4f),
				Randomizer.Current.Get(-0.4f, +0.4f));

			Add<MoveSpriteReflectingAtBorder>();
			Add<RotateSpriteByRotationSpeed>();
		}
	}
}