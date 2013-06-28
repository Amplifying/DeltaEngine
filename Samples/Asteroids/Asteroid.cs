using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.Sprites;

namespace Asteroids
{
	/// <summary>
	/// representing an asteroid that can be destroyed, causing it to split into two objects of half the
	/// size at first. Those of half size can split again and those of third part the size shall vanish.
	/// </summary>
	public class Asteroid : Sprite
	{
		public Asteroid(Randomizer randomizer, GameLogic gameLogic, int sizeModifier = 1)
			: base(
				ContentLoader.Load<Image>(AsteroidTextureName), CreateDrawArea(randomizer, sizeModifier))
		{
			this.gameLogic = gameLogic;
			this.sizeModifier = sizeModifier;
			RenderLayer = (int)AsteroidsRenderLayer.Asteroids;
			Add(new SimplePhysics.Data
			{
				Gravity = Point.Zero,
				Velocity = new Point(randomizer.Get(.03f, .15f), randomizer.Get(.03f, .15f)),
				RotationSpeed = randomizer.Get(.1f, 50)
			});

			Add(0.0f);
			Start<SimplePhysics.BounceOffScreenEdges>();
			Start<SimplePhysics.Rotate>();
		}

		private const string AsteroidTextureName = "asteroid";

		private static Rectangle CreateDrawArea(Randomizer randomizer, int sizeModifier)
		{
			var randomPosition = new Point(randomizer.Get(-1, 1) > 0 ? 0.2f : 0.8f,
				randomizer.Get(-1, 1) > 0 ? 0.2f : 0.8f);
			var modifiedSize = new Size(.1f / sizeModifier);
			return new Rectangle(randomPosition, modifiedSize);
		}

		public readonly int sizeModifier;
		private readonly GameLogic gameLogic;

		public void Fracture()
		{
			if (sizeModifier < 3)
				gameLogic.CreateAsteroidsAtPosition(DrawArea.Center, sizeModifier + 1);

			gameLogic.IncrementScore(1);
			IsActive = false;
		}
	}
}