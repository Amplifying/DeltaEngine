using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Simple fadeout particle effect, which is based on a sprite and disposes it after timout.
	/// </summary>
	public class FadeoutEffect : Sprite
	{
		public FadeoutEffect(Image image, Rectangle drawArea, float timeout = 1.0f)
			: base(image, drawArea)
		{
			this.timeout = timeout;
		}

		private readonly float timeout;

		protected override void Render(Renderer renderer, Time time)
		{
			Color.AlphaValue = 1.0f - elapsed / timeout;
			base.Render(renderer, time);
			elapsed += time.CurrentDelta;
			if (elapsed >= timeout)
				Dispose();
		}

		private float elapsed;
	}
}