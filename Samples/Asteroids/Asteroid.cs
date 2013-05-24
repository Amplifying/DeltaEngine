using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.Sprites;

namespace Asteroids
{
	/// <summary>
	/// representing an asteroid that can be destroyed, causing it to split into two objects of half the
	/// size at first. Those of half size can split again and those of quarter size shall vanish.
	/// </summary>
	public class Asteroid : Sprite
	{
		public Asteroid(ContentLoader content, Randomizer randomizer,
			AsteroidsGame game)
			: this(content, randomizer, 1, game) {}

		public Asteroid(ContentLoader content, Randomizer randomizer,
			int sizeModifier, AsteroidsGame game)
			: base(content.Load<Image>("asteroid"),
				new Rectangle(new Point(randomizer.Get(-1, 1) > 0 ? 0.2f : 0.8f, 
					randomizer.Get(-1, 1) > 0 ? 0.2f : 0.8f),
				new Size(.1f / sizeModifier)), Color.White)
		{
			//var physicsDataComponent = new SimplePhysics.Data();
			Add(new SimplePhysics.Data()
			{
				Gravity = Point.Zero,
				Velocity = new Point(randomizer.Get(.03f, .15f), randomizer.Get(.03f, .15f)),
				RotationSpeed = randomizer.Get(.1f, 50)
			});
			this.game = game;
			Add(0.0f);
			this.sizeModifier = sizeModifier;
			Add<SimplePhysics.BounceOffScreenEdges>();
			Add<SimplePhysics.Rotate>();
			RenderLayer = (int)Constants.RenderLayer.Asteroids;
		}

		public readonly int sizeModifier;
		private readonly AsteroidsGame game;

		public void Fracture()
		{
			if (sizeModifier < 3)
				game.CreateAsteroidsAtPosition(DrawArea.Center, sizeModifier + 1);
			IsActive = false;
		}
	}
}