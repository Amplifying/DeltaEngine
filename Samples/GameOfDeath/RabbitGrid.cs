using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;
using GameOfDeath.Items;

namespace GameOfDeath
{
	/// <summary>
	/// Derived from the GameOfLife and handles the rabbit logic.
	/// </summary>
	public class RabbitGrid : GameOfLife
	{
		public RabbitGrid(ContentLoader content, ScreenSpace screen)
			: base(GetColumns(screen.Viewport), GetRows(screen.Viewport))
		{
			LoadContent(content);
			InitializeRabbits();
		}

		private static int GetColumns(Rectangle viewport)
		{
			initialSize.Width = viewport.Left;
			return (int)((viewport.Right - viewport.Left + CellSize.Width) / CellSize.Width) - 1;
		}

		private static int GetRows(Rectangle viewport)
		{
			initialSize.Height = viewport.Top;
			return (int)((viewport.Bottom - viewport.Top + CellSize.Height) / CellSize.Height) - 2;
		}

		private static Size initialSize;
		public static readonly Size CellSize = new Size(0.05f);

		private void LoadContent(ContentLoader content)
		{
			rabbitImage = content.Load<Image>("Rabbit");
			deadRabbitImage = content.Load<Image>("DeadRabbit");
			malletHitSound = content.Load<Sound>("MalletHit");
			malletBloodImage = content.Load<Image>("BloodSplatter");
			gameOverImage = content.Load<Image>("GameOver");
		}

		private Image rabbitImage;
		private Image deadRabbitImage;
		private Sound malletHitSound;
		private Image malletBloodImage;
		private Image gameOverImage;

		private void InitializeRabbits()
		{
			rabbits = new Rabbit[width,height];
			growCell = new float[width,height];
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					rabbits[x, y] = new Rabbit(rabbitImage, CalculatePosition(x, y));
		}

		private Rabbit[,] rabbits;
		private float[,] growCell;

		private static Point CalculatePosition(int x, int y)
		{
			return new Point(initialSize.Width + (x + 1) * CellSize.Width,
				initialSize.Height + (y + 2) * CellSize.Height);
		}

		public override bool ShouldSurvive(int x, int y)
		{
			return base[x, y] ||
				GetNumberOfNeighbours(x, y) > (Time.Current.Milliseconds > 30000 ? 2 : 3);
		}

		public override void Run()
		{
			CheckForPayday();
			GrowRabbits();
			if (!IsGameOver() && Time.Current.CheckEvery(GetTimeStep()))
				RandomlySpawn();
		}

		private void CheckForPayday()
		{
			if (Time.Current.CheckEvery(PaydayInterval) && MoneyEarned != null)
				MoneyEarned(Payday);
		}

		private const float PaydayInterval = 1.0f;
		private const int Payday = 1;
		public event Action<int> MoneyEarned;

		private void GrowRabbits()
		{
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					if (base[x, y])
						GrowRabbit(x, y);
		}

		private void GrowRabbit(int x, int y)
		{
			if (rabbits[x, y].IsDead)
				rabbits[x, y].SetHealth(GetCurrentNewRabbitHealth());

			if (growCell[x, y] < 1.0f)
				growCell[x, y] += GrowSpeed * Time.Current.Delta;

			rabbits[x, y].IsVisible = true;
			rabbits[x, y].Scale = growCell[x, y];
		}

		private static float GetCurrentNewRabbitHealth()
		{
			return InitialRabbitHealth + Time.Current.Milliseconds / IncreaseRabbitHealthEveryMs;
		}

		private const float InitialRabbitHealth = 1;
		private const float IncreaseRabbitHealthEveryMs = 2.0f * 1000;
		private const float GrowSpeed = 0.66f;

		private static float GetTimeStep()
		{
			return MathExtensions.Max(1.5f - 0.5f * Time.Current.Milliseconds / 60000.0f, 0.1f);
		}

		private void RandomlySpawn()
		{
			int x = Randomizer.Current.Get(0, width);
			int y = Randomizer.Current.Get(0, height);
			base[x, y] = true;
			if (x + 1 < width && Randomizer.Current.Get(0, 2) == 1)
				base[x + 1, y] = true;

			if (y + 1 < height && Randomizer.Current.Get(0, 2) == 1)
				base[x, y + 1] = true;
		}

		public void DoDamage(Point positionHit, float sizeOfImpact, float damage)
		{
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					if (CalculatePosition(x, y).DistanceTo(positionHit) < sizeOfImpact && base[x, y])
						DamageRabbit(x, y, damage);
		}

		private void DamageRabbit(int x, int y, float damage)
		{
			ShowMalletHit(x, y, damage);
			rabbits[x, y].DoDamage(damage);
			if (rabbits[x, y].IsDead)
				KillRabbit(x, y);
		}

		private void ShowMalletHit(int x, int y, float damage)
		{
			if (damage != Mallet.DefaultDamage)
				return;

			malletHitSound.Play(0.5f);
			CreateMalletBlood(rabbits[x, y].RabbitSprite.DrawArea.Center);
		}

		private void CreateMalletBlood(Point center)
		{
			var drawArea = Rectangle.FromCenter(center, MalletBloodSize);
			var blood = new Sprite(malletBloodImage, drawArea);
			blood.Add<FinalTransition>().Add(new Transition.Duration(1.0f)).Add(
				new Transition.FadingColor(blood.Color));
		}

		private static readonly Size MalletBloodSize = new Size(0.09f);

		private void KillRabbit(int x, int y)
		{
			rabbits[x, y].IsVisible = false;
			base[x, y] = false;
			growCell[x, y] = 0.0f;
			new DeadRabbit(deadRabbitImage, rabbits[x, y].RabbitSprite.DrawArea);
			AddToScoreAndMoney();
		}

		private void AddToScoreAndMoney()
		{
			if (RabbitKilled != null)
				RabbitKilled();

			if (MoneyEarned != null)
				MoneyEarned(MoneyPerDeadRabbit);
		}

		public event Action RabbitKilled;
		private const int MoneyPerDeadRabbit = 2;

		public bool IsGameOver()
		{
			if (!gameOver)
				CheckPopulation();

			return gameOver;
		}

		private bool gameOver;

		private void CheckPopulation()
		{
			if (!IsOverPopulated())
				return;

			gameOver = true;
			new Sprite(gameOverImage,
				Rectangle.FromCenter(Point.Half,
					gameOverImage.PixelSize / Scoreboard.QuadraticFullscreenSize));
			if (GameOver != null)
				GameOver();
		}

		public event Action GameOver;

		public bool IsOverPopulated()
		{
			return NumberOfActiveRabbits > width * height * OverPopulationPercentage;
		}

		private const float OverPopulationPercentage = 0.75f;

		private int NumberOfActiveRabbits
		{
			get
			{
				int activeRabbits = 0;
				for (int x = 0; x < width; x++)
					for (int y = 0; y < height; y++)
						if (rabbits[x, y].IsVisible)
							activeRabbits++;

				return activeRabbits;
			}
		}
	}
}