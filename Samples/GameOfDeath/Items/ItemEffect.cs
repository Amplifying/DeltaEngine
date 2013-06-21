using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Each item displays an fading out effect once applied.
	/// </summary>
	public class ItemEffect : Sprite
	{
		public ItemEffect(Image image, Rectangle drawArea, float duration)
			: base(image, drawArea)
		{
			RenderLayer = 1;
			Add(new Damage { Interval = 0.25f });
			Add<DoDamage>();
			Fadeout(duration);
		}

		private void Fadeout(float duration)
		{
			Add<FinalTransition>().Add(new Transition.Duration(duration)).Add(
				new Transition.FadingColor(Color));
		}

		public float DamageInterval
		{
			get { return Get<Damage>().Interval; }
			set { Get<Damage>().Interval = value; }
		}

		public Action DoDamage
		{
			get { return Get<Damage>().DoDamage; }
			set { Get<Damage>().DoDamage = value; }
		}
	}
}