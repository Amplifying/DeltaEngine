using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering;
using GameOfDeath.Items;

namespace GameOfDeath
{
	/// <summary>
	/// Derived from the GameOfLife and connects all the other parts together for this game.
	/// </summary>
	public class Game : GameOfLife
	{
		public Game(Intro intro, Content content, Renderer renderer, Time time, Score score)
			: base(GetColumns(renderer.Screen.Viewport), GetRows(renderer.Screen.Viewport))
		{
			rabbitImage = content.Load<Image>("Rabbit");
			deadRabbitImage = content.Load<Image>("DeadRabbit");
			gameOverImage = content.Load<Image>("GameOver");
			malletHitSound = content.Load<Sound>("MalletHit");
			malletBloodImage = content.Load<Image>("BloodSplatter");
			this.renderer = renderer;
			this.time = time;
			this.score = score;
			InitializeRabbits();
		}

		private static int GetColumns(Rectangle viewport)
		{
			initialLeft = viewport.Left;
			return (int)((viewport.Right - viewport.Left + CellSize.Width) / CellSize.Width) - 1;
		}

		private static float initialLeft;

		private static int GetRows(Rectangle viewport)
		{
			initialTop = viewport.Top;
			return (int)((viewport.Bottom - viewport.Top + CellSize.Height) / CellSize.Height) - 2;
		}

		private static float initialTop;

		public static readonly Size CellSize = new Size(0.05f);
		private readonly Image rabbitImage;
		private readonly Image deadRabbitImage;
		private readonly Image gameOverImage;
		private readonly Sound malletHitSound;
		private readonly Image malletBloodImage;
		private readonly Renderer renderer;
		private readonly Time time;
		private readonly Score score;

		private void InitializeRabbits()
		{
			rabbits = new Rabbit[width,height];
			growCell = new float[width, height];
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					renderer.Add(CreateRabbit(x, y));
		}

		private Rabbit[,] rabbits;
		private float[,] growCell;

		private Renderable CreateRabbit(int x, int y)
		{
			return rabbits[x, y] = new Rabbit(rabbitImage, CalclatePosition(x, y));
		}

		private Point CalclatePosition(int x, int y)
		{
			return new Point(initialLeft + (x + 1) * CellSize.Width,
				initialTop + (y + 2) * CellSize.Height);
		}

		private float CurrentNewRabbitHealth
		{
			get { return InitialRabbitHealth + time.Milliseconds / IncreaseRabbitHealthEveryMs; }
		}
		private const float InitialRabbitHealth = 1;
		private const float IncreaseRabbitHealthEveryMs = 2.0f * 1000;

		public override bool ShouldSurvive(int x, int y)
		{
			return base[x, y] || GetNumberOfNeighbours(x, y) > (time.Milliseconds > 30000 ? 2 : 3);
		}

		public override void Run()
		{
			GrowRabbits();
			if (time.CheckEvery(GiveMoneyTimeStep))
				score.CurrentMoney++;

			if (IsGameOver() || !time.CheckEvery(UpdateTimeStep))
				return;

			base.Run();
			RandomlySpawnCellsForRabbitBirth();
		}

		private float UpdateTimeStep
		{
			get { return MathExtensions.Max(1.5f - 0.5f * time.Milliseconds / 60000.0f, 0.1f); }
		}

		private const float GiveMoneyTimeStep = 1.0f;

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
				rabbits[x, y].SetHealth(CurrentNewRabbitHealth);

			if (growCell[x, y] < 1.0f)
				growCell[x, y] += GrowSpeed * time.CurrentDelta;

			rabbits[x, y].IsVisible = true;
			rabbits[x, y].Scale = growCell[x, y];
		}

		private const float GrowSpeed = 0.66f;

		public bool IsGameOver()
		{
			if (gameOver)
				return true;

			int activeRabbits = NumberOfActiveRabbits;
			if (activeRabbits > width * height * 3/ 4)
			{
				gameOver = true;
				renderer.Add(new Sprite(gameOverImage,
					Rectangle.FromCenter(Point.Half, gameOverImage.PixelSize / Score.QuadraticFullscreenSize)));
			}

			return gameOver;
		}

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

		private bool gameOver;

		private void RandomlySpawnCellsForRabbitBirth()
		{
			int x = Random.Get(0, width);
			int y = Random.Get(0, height);
			base[x, y] = true;
			if (x + 1 < width && Random.Get(0, 2) == 1)
				base[x + 1, y] = true;
			if (y + 1 < height && Random.Get(0, 2) == 1)
				base[x, y + 1] = true;
		}

		private static readonly Randomizer Random = new PseudoRandom();

		public void DoDamage(Point positionHit, float sizeOfImpact, float damage)
		{
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					if (CalclatePosition(x, y).DistanceTo(positionHit) < sizeOfImpact && base[x, y])
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
			renderer.Add(new FadeoutEffect(malletBloodImage,
				Rectangle.FromCenter(rabbits[x, y].DrawArea.Center, new Size(0.09f))));
		}

		private void KillRabbit(int x, int y)
		{
			score.CurrentKills++;
			score.CurrentMoney += 2;
			rabbits[x, y].IsVisible = false;
			base[x, y] = false;
			growCell[x, y] = 0.0f;
			renderer.Add(new DeadRabbit(deadRabbitImage, rabbits[x, y].DrawArea));
		}
	}
}