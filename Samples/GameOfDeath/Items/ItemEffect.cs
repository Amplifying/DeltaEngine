using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Each item displays an fading out effect once applied.
	/// </summary>
	public class ItemEffect : FadeoutEffect
	{
		public ItemEffect(Image image, Rectangle drawArea, float duration)
			: base(image, drawArea, duration)
		{
			RenderLayer = 1;
		}

		protected override void Render(Renderer renderer, Time time)
		{
			if (IsVisible && time.CheckEvery(DoDamageEvery))
				DidDamage();

			base.Render(renderer, time);
		}

		public float DoDamageEvery = 0.25f;
		public Action DidDamage;
	}
}