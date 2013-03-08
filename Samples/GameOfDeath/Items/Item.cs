using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Base for all items, manages the most important values, which need to be implemented.
	/// </summary>
	public abstract class Item : Sprite
	{
		protected Item(Image image, Image imageEffect, Sound soundEffect, Point initialPosition)
			: base(image, Rectangle.FromCenter(initialPosition, Size.Zero))
		{
			this.imageEffect = imageEffect;
			this.soundEffect = soundEffect;
			RenderLayer = MaxRenderLayer;
		}

		private readonly Image imageEffect;
		private readonly Sound soundEffect;

		public virtual void UpdatePosition(Point newPosition)
		{
			DrawArea = Rectangle.FromCenter(newPosition, Image.PixelSize / Score.QuadraticFullscreenSize);
		}

		protected abstract float ImpactSize { get; }
		protected abstract float ImpactTime { get; }
		protected abstract float Damage { get; }
		protected abstract float DoDamageEvery { get; }
		public abstract int Cost { get; }

		public virtual ItemEffect CreateEffect(Point position, Game game)
		{
			soundEffect.Play();
			var size = new Size(ImpactSize * 2);
			if (imageEffect == null)
				size.Width *= DrawArea.Size.AspectRatio;
			return new ItemEffect(imageEffect ?? Image, position, size, ImpactTime)
			{
				DoDamageEvery = DoDamageEvery,
				DoDamage = () => game.DoDamage(position, ImpactSize, Damage),
			};
		}
	}
}