using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering;
using GameOfDeath.Items;

namespace GameOfDeath
{
	//
	/// <summary>
	/// Derived from the GameOfLife and handles the rabbits logics.
	/// </summary>
	public class RabbitsGrid : GameOfLife
	{
		public RabbitsGrid(Content content, Renderer renderer, Time time, Score score)
			: base(GetColumns(renderer.Screen.Viewport), GetRows(renderer.Screen.Viewport))
		{
			rabbitImage = content.Load<Image>("Rabbit");
			deadRabbitImage = content.Load<Image>("DeadRabbit");
			malletHitSound = content.Load<Sound>("MalletHit");
			malletBloodImage = content.Load<Image>("BloodSplatter");
			this.renderer = renderer;
			this.score = score;
			this.time = time;
		}

		private readonly Renderer renderer;
		private readonly Score score;
		private readonly Time time;

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
		private readonly Image rabbitImage;
		private readonly Image malletBloodImage;
		private readonly Image deadRabbitImage;
		private readonly Sound malletHitSound;

		public void InitializeRabbits()
		{
			rabbits = new Rabbit[width, height];
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

		private static Point CalclatePosition(int x, int y)
		{
			return new Point(initialSize.Width + (x + 1) * CellSize.Width,
				initialSize.Height + (y + 2) * CellSize.Height);
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

		public void Grow()
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

		public void RandomlySpawn()
		{
			var random = new PseudoRandom();
			int x = random.Get(0, width);
			int y = random.Get(0, height);
			base[x, y] = true;
			if (x + 1 < width && random.Get(0, 2) == 1)
				base[x + 1, y] = true;
			if (y + 1 < height && random.Get(0, 2) == 1)
				base[x, y + 1] = true;
		}

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

		public bool IsOverPopulated()
		{
			int activeRabbits = NumberOfActiveRabbits;
			return (activeRabbits > width * height * 3 / 4);
		}
	}
}
