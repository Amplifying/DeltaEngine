using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;

namespace GameOfDeath
{
	/// <summary>
	/// Holds the rabbit sprite and healthbar
	/// </summary>
	public class Rabbit : Entity2D
	{
		public Rabbit(Image image, Point position) :base(new Rectangle(position, image.PixelSize))
		{
			CreateRabbitSprite(image, position);
			CreateRabbitHealthBar();
			Add(new RabbitHealth());
			Start<MoveRabbit>();
			Start<UpdateRabbitHealthBar>();
		}

		private void CreateRabbitSprite(Image image, Point position)
		{
			var rabbitSprite = new RabbitSprite(image, position);
			Add(rabbitSprite);
		}

		private void CreateRabbitHealthBar()
		{
			var healthBar = new FilledRect(new Rectangle(), Color.Red) { Visibility = Visibility.Hide };
			healthBar.Stop<Polygon.RenderOutline>();
			var rabbitHealthBar = healthBar;
			Add(rabbitHealthBar);
		}

		public void SetHealth(float initialHealth)
		{
			var data = Get<RabbitHealth>();
			data.CurrentHealth = initialHealth;
			data.MaxHealth = initialHealth;
			IsVisible = true;
		}

		public bool IsVisible
		{
			get { return RabbitSprite.Visibility == Visibility.Show; }
			set
			{
				RabbitSprite.Visibility = value ? Visibility.Show : Visibility.Hide;
				RabbitHealthBar.Visibility = value ? Visibility.Show : Visibility.Hide;
			}
		}

		public float HealthPercentage
		{
			get
			{
				var data = Get<RabbitHealth>();
				return data.CurrentHealth / data.MaxHealth;
			}
		}

		public float Scale
		{
			set
			{
				var sprite = RabbitSprite;
				sprite.DrawArea = Rectangle.FromCenter(sprite.DrawArea.Center, sprite.OriginalSize * value);
			}
		}

		public void DoDamage(float damage)
		{
			Get<RabbitHealth>().CurrentHealth -= damage;
		}

		public bool IsDead
		{
			get { return Get<RabbitHealth>().CurrentHealth <= 0; }
		}

		public RabbitSprite RabbitSprite
		{
			get { return Get<RabbitSprite>(); }
		}

		public FilledRect RabbitHealthBar
		{
			get { return Get<FilledRect>(); }
		}
	}
}