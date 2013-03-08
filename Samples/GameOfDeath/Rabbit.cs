using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace GameOfDeath
{
	/// <summary>
	/// Like BouncingLogo rabbits are just sprites bouncing around in a limited area box.
	/// </summary>
	public class Rabbit : Sprite
	{
		public Rabbit(Image image, Point position)
			: base(image, Rectangle.One)
		{
			originalSize = image.PixelSize / Score.QuadraticFullscreenSize;
			DrawArea = Rectangle.FromCenter(position, originalSize);
			velocity = new Point(Random.Get(-0.035f, 0.035f), Random.Get(-0.025f, 0.025f));
			boundingBox = Rectangle.FromCenter(position, originalSize * 1.2f);
			IsVisible = false;
		}

		private readonly Size originalSize;
		private static readonly Randomizer Random = new PseudoRandom();
		private Point velocity;
		private Rectangle boundingBox;

		public void SetHealth(float initialHealth)
		{
			IsVisible = true;
			currentHealth = initialHealth;
			maxHealth = initialHealth;
		}

		private float currentHealth;
		private float maxHealth;

		public float Scale
		{
			set { DrawArea = Rectangle.FromCenter(DrawArea.Center, originalSize * value); }
		}

		protected override void Render(Renderer renderer, Time time)
		{
			DrawArea.Center += velocity * time.CurrentDelta;
			velocity.ReflectIfHittingBorder(DrawArea, boundingBox);
			DrawHealthBar(renderer);
			base.Render(renderer, time);
		}

		private void DrawHealthBar(Renderer renderer)
		{
			renderer.DrawRectangle(HealthBox, HealthColor);
		}

		protected Rectangle HealthBox
		{
			get
			{
				return Rectangle.FromCenter(DrawArea.Center.X, DrawArea.Top - 0.005f,
					HealthPercentage * DrawArea.Width * 0.5f, HealthBarHeight);
			}
		}
		protected float HealthPercentage
		{
			get { return currentHealth / maxHealth; }
		}
		private const float HealthBarHeight = 5.0f / 1920;

		private Color HealthColor
		{
			get { return HealthPercentage < 0.25f ? Red : HealthPercentage < 0.5f ? Yellow : Green; }
		}
		private static readonly Color Red = new Color(96, 0, 0, 128);
		private static readonly Color Yellow = new Color(96, 96, 0, 128);
		private static readonly Color Green = new Color(0, 96, 0, 128);

		public void DoDamage(float damage)
		{
			currentHealth -= damage;
		}

		public bool IsDead
		{
			get { return currentHealth <= 0; }
		}
	}
}