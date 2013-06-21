using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.Sprites;

namespace LogoApp
{
	/// <summary>
	/// Colored Delta Engine logo which spins and bounces around the screen
	/// </summary>
	public class BouncingLogo : Sprite
	{
		public BouncingLogo(ContentLoader content)
			: base(content.Load<Image>("DeltaEngineLogo"), LogoDrawArea, Color.GetRandomColor())
		{
			Randomizer random = Randomizer.Current;
			Rotation = random.Get(0, 360);
			Add(new SimplePhysics.Data
			{
				RotationSpeed = random.Get(-50, 50),
				Velocity = new Point(random.Get(-0.4f, 0.4f), random.Get(-0.4f, 0.4f))
			});
			Add<SimplePhysics.BounceOffScreenEdges, SimplePhysics.Rotate>();
		}

		private static readonly Rectangle LogoDrawArea = new Rectangle(0.4f, 0.4f, 0.2f, 0.2f);
	}
}