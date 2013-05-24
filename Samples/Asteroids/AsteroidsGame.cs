using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Sprites;

namespace Asteroids
{
	/// <summary>
	/// class to be used for running the Asteroids-game, initializing entities and taking care of
	/// interaction that is not done by any EntityHandler
	/// </summary>
	public class AsteroidsGame : Entity
	{
		public AsteroidsGame(ContentLoader content, InputCommands input)
		{
			AsteroidsGame.content = content;
			this.entitySystem = entitySystem;
			this.input = input;
			Add<GameLogicsHandler>();
			existantAsteroids = new List<Asteroid>();
			existantProjectiles = new List<Projectile>();
			random = new PseudoRandom();
			score = 0;
			SetUpBackground();
			player = new PlayerShip(content, this);
			SetUpInput();
			CreateRandomAsteroids(1);
			CreateRandomAsteroids(1, 2);
		}

		public static ContentLoader content;
		internal readonly EntitySystem entitySystem;
		public readonly InputCommands input;
		public List<Asteroid> existantAsteroids;
		public List<Projectile> existantProjectiles;
		internal readonly Randomizer random;
		public PlayerShip player;
		internal List<Asteroid> asteroidsToCreate = new List<Asteroid>();
		internal int score;

		private static void SetUpBackground()
		{
			new Sprite(content.Load<Image>("black-background"), new Rectangle(Point.Zero, new Size(1)));
		}

		private void SetUpInput()
		{
			input.Add(Key.W, State.Pressed, ControlUp);
			input.Add(Key.W, State.Pressing, ControlUp);

			input.Add(Key.A, State.Pressed, ControlLeft);
			input.Add(Key.A, State.Pressing, ControlLeft);

			input.Add(Key.D, State.Pressed, ControlRight);
			input.Add(Key.D, State.Pressing, ControlRight);

			input.Add(Key.CursorUp, State.Pressed, ControlUp);
			input.Add(Key.CursorUp, State.Pressing, ControlUp);

			input.Add(Key.CursorLeft, State.Pressed, ControlLeft);
			input.Add(Key.CursorLeft, State.Pressing, ControlLeft);

			input.Add(Key.CursorRight, State.Pressed, ControlRight);
			input.Add(Key.CursorRight, State.Pressing, ControlRight);

			input.Add(Key.Space, State.Pressing, ControlFire);
			input.Add(Key.Space, State.Releasing, ControlHoldFire);
		}

		public void ControlUp()
		{
			player.ShipAccelerate();
		}

		public void ControlLeft()
		{
			player.SteerLeft();
		}

		public void ControlRight()
		{
			player.SteerRight();
		}

		public void ControlFire()
		{
			player.IsFireing = true;
		}

		public void ControlHoldFire()
		{
			player.IsFireing = false;
		}

		public void CreateRandomAsteroids(int howMany, int sizeMod = 1)
		{
			for (int asteroidCount = 0; asteroidCount < howMany; asteroidCount ++)
			{
				var asteroid = new Asteroid(content, random, sizeMod, this);
				asteroidsToCreate.Add(asteroid);
			}
		}

		public void CreateAsteroidsAtPosition(Point position, int sizeMod = 1, int howMany = 2)
		{
			for (int asteroidCount = 0; asteroidCount < howMany; asteroidCount++)
			{
				var asteroid = new Asteroid(content, random, sizeMod, this);
				asteroid.DrawArea = new Rectangle(position, asteroid.DrawArea.Size);
				asteroidsToCreate.Add(asteroid);
			}
		}

		public void GameOver()
		{
			player.IsActive = false;
		}
	}
}