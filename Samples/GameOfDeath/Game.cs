using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace GameOfDeath
{
	/// <summary>
	/// Connects all the other parts together for Game Of Death.
	/// </summary>
	public sealed class Game : Runner
	{
		public Game(Content content, Renderer renderer, Time time, Score score, RabbitsGrid rabbits)
		{
			gameOverImage = content.Load<Image>("GameOver");
			this.time = time;
			this.score = score;
			this.renderer = renderer;
			this.rabbits = rabbits;
			rabbits.InitializeRabbits();
		}

		private readonly Image gameOverImage;
		private readonly Time time;
		private readonly Score score;
		private readonly Renderer renderer;
		private readonly RabbitsGrid rabbits;

		public void Run()
		{
			rabbits.Grow();
			if (time.CheckEvery(GiveMoneyTimeStep))
				score.CurrentMoney++;

			if (IsGameOver() || !time.CheckEvery(UpdateTimeStep))
				return;

			rabbits.RandomlySpawn();
		}

		private float UpdateTimeStep
		{
			get { return MathExtensions.Max(1.5f - 0.5f * time.Milliseconds / 60000.0f, 0.1f); }
		}

		private const float GiveMoneyTimeStep = 1.0f;

		public bool IsGameOver()
		{
			if (gameOver)
				return true;

			if (rabbits.IsOverPopulated())
			{
				gameOver = true;
				renderer.Add(new Sprite(gameOverImage,
					Rectangle.FromCenter(Point.Half, gameOverImage.PixelSize / Score.QuadraticFullscreenSize)));
			}
			return gameOver;
		}

		private bool gameOver;

		public void DoDamage(Point positionHit, float sizeOfImpact, float damage)
		{
			rabbits.DoDamage(positionHit, sizeOfImpact, damage);
		}
	}
}