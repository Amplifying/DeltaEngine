using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering;

namespace Breakout
{
	/// <summary>
	/// All the bricks for each level are initialized and updated here.
	/// </summary>
	public class Level : IDisposable
	{
		public Level(Content content, Renderer renderer, Score score)
		{
			brickImage = content.Load<Image>("Brick");
			explosionImage = content.Load<Image>("Explosion");
			explosionSound = content.Load<Sound>("BrickExplosion");
			lostBallSound = content.Load<Sound>("LostBall");
			this.renderer = renderer;
			this.score = score;
			Initialize();
		}

		private readonly Image brickImage;
		private readonly Image explosionImage;
		private readonly Sound explosionSound;
		private readonly Sound lostBallSound;
		protected readonly Renderer renderer;
		private readonly Score score;

		private void Initialize()
		{
			rows = score.Level + 1;
			columns = score.Level + 1;
			bricks = new Sprite[rows, columns];
			brickWidth = 1.0f / rows;
			brickHeight = 0.5f / columns;
			CreateBricks();
		}

		private float brickWidth;
		private float brickHeight;
		protected int rows;
		protected int columns;
		protected Sprite[,] bricks;

		private void CreateBricks()
		{
			for (int x = 0; x < rows; x++)
				for (int y = 0; y < columns; y++)
					bricks[x, y] = CreateBrick(x, y);
		}

		private Sprite CreateBrick(int x, int y)
		{
			var brick = new Sprite(brickImage, GetBounds(x, y), GetBrickColor(x, y));
			renderer.Add(brick);
			return brick;
		}

		private Rectangle GetBounds(int x, int y)
		{
			return new Rectangle(x * brickWidth, y * brickHeight, brickWidth, brickHeight);
		}

		private Color GetBrickColor(int x, int y)
		{
			if (score.Level <= 1)
				return GetLevelOneBrickColor(x, y);

			if (score.Level == 2)
				return GetLevelTwoBrickColor(y);

			if (score.Level == 3)
				return GetLevelThreeBrickColor(x, y);

			if (score.Level == 4)
				return GetLevelFourBrickColor(x, y);

			return GetLevelFiveOrAboveBrickColor(x, y);
		}

		private static Color GetLevelOneBrickColor(int x, int y)
		{
			return x + y == 1 ? Color.Green : Color.Orange;
		}

		private static Color GetLevelTwoBrickColor(int y)
		{
			if (y == 0)
				return new Color(0.25f, 0.25f, 0.25f);

			return y == 1 ? Color.Red : Color.Gold;
		}

		private static Color GetLevelThreeBrickColor(int x, int y)
		{
			return LevelThreeColors[(x * 4 + y) % LevelThreeColors.Length];
		}

		private static readonly Color[] LevelThreeColors = new[] { Color.Yellow, Color.Teal,
			Color.Green, Color.LightBlue, Color.Teal };

		private static Color GetLevelFourBrickColor(int x, int y)
		{
			return new Color(x * 0.2f + 0.1f, 0.2f, (x + y / 2) * 0.15f + 0.2f);
		}

		private static Color GetLevelFiveOrAboveBrickColor(int x, int y)
		{
			return new Color(0.9f - x * 0.15f, 0.5f, (x + y / 2) * 0.1f + 0.2f);
		}

		public void InitializeNextLevel()
		{
			Dispose();
			score.NextLevel();
			Initialize();
		}

		public void Dispose()
		{
			for (int x = 0; x < rows; x++)
				for (int y = 0; y < columns; y++)
					bricks[x, y].Dispose();
		}

		public int BricksLeft
		{
			get
			{
				var bricksAlive = 0;
				for (int x = 0; x < rows; x++)
					for (int y = 0; y < columns; y++)
						if (bricks[x, y].IsVisible)
							bricksAlive++;

				return bricksAlive;
			}
		}

		public Sprite GetBrickAt(float x, float y)
		{
			var brickIndexX = (int)(x / brickWidth);
			var brickIndexY = (int)(y / brickHeight);
			if (brickIndexX < 0 || brickIndexX >= rows || brickIndexY < 0 || brickIndexY >= columns ||
				!bricks[brickIndexX, brickIndexY].IsVisible)
				return null;

			return bricks[brickIndexX, brickIndexY];
		}

		public void Explode(Sprite brick)
		{
			score.IncreasePoints();
			brick.Dispose();
			renderer.Add(new FadeoutEffect(explosionImage, Rectangle.FromCenter(brick.DrawArea.Center, 
				new Size(brickWidth))));
			explosionSound.Play();
		}

		public void LifeLost(Point ballLostPosition)
		{
			score.LifeLost();
			renderer.Add(new FadeoutEffect(explosionImage, Rectangle.FromCenter(ballLostPosition, 
				new Size(0.2f))));
			lostBallSound.Play();
		}
	}
}