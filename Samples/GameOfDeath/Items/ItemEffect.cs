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
		public ItemEffect(Image image, Point position, Size size, float duration)
			: base(image, position, size, duration)
		{
			RenderLayer = DefaultRenderLayer + 1;
		}

		public Action DoDamage;
		public float DoDamageEvery = 0.25f;

		protected override void Render(Renderer renderer, Time time)
		{
			if (IsVisible && time.CheckEvery(DoDamageEvery))
				DoDamage();

			base.Render(renderer, time);
		}
	}
}