using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering.Sprites;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Base for all items, manages the most important values, which need to be implemented.
	/// </summary>
	public abstract class Item : Sprite
	{
		protected Item(Image image, Image imageEffect, Sound soundEffect)
			: base(image, Rectangle.FromCenter(Point.Half, Size.Zero))
		{
			this.imageEffect = imageEffect;
			this.soundEffect = soundEffect;
			RenderLayer = (int)GameCoordinator.RenderLayers.Items;
		}

		private readonly Image imageEffect;
		private readonly Sound soundEffect;

		public virtual void UpdatePosition(Point newPosition)
		{
			DrawArea = Rectangle.FromCenter(newPosition,
				Image.PixelSize / Scoreboard.QuadraticFullscreenSize);
		}

		protected abstract float ImpactSize { get; }
		protected abstract float ImpactTime { get; }
		protected abstract float Damage { get; }
		protected abstract float DamageInterval { get; }
		public abstract int Cost { get; }

		public virtual ItemEffect CreateEffect(Point position)
		{
			soundEffect.Play();
			var size = new Size(ImpactSize * 2);
			if (imageEffect == null)
				size.Width *= DrawArea.Size.AspectRatio;

			return new ItemEffect(imageEffect ?? Image, Rectangle.FromCenter(position, size), ImpactTime)
			{
				DamageInterval = DamageInterval,
				DoDamage = () => DoDamage(position, ImpactSize, Damage),
			};
		}

		public Action<Point, float, float> DoDamage;
	}
}