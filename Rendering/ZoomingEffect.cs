using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Simple zooming particle effect, which is based on a sprite and disposes it after timout.
	/// </summary>
	public class ZoomingEffect : Sprite
	{
		public ZoomingEffect(Image image, Rectangle startDrawArea, Rectangle endDrawArea,
			float timeout = 1.0f)
			: base(image, startDrawArea)
		{
			this.startDrawArea = startDrawArea;
			this.endDrawArea = endDrawArea;
			this.timeout = timeout;
		}

		private readonly Rectangle startDrawArea;
		private readonly Rectangle endDrawArea;
		private readonly float timeout;

		protected override void Render(Renderer renderer, Time time)
		{
			MoveSprite();
			Color.AlphaValue = 1.0f - elapsed / timeout;
			base.Render(renderer, time);
			CheckIfTimeToDispose(time);
		}

		private void MoveSprite()
		{
			float percentage = elapsed / timeout;
			Point topLeft = Point.Lerp(startDrawArea.TopLeft, endDrawArea.TopLeft, percentage);
			Size size = Size.Lerp(startDrawArea.Size, endDrawArea.Size, percentage);
			DrawArea = new Rectangle(topLeft, size);
		}

		private float elapsed;

		private void CheckIfTimeToDispose(Time time)
		{
			elapsed += time.CurrentDelta;
			if (elapsed >= timeout)
				Dispose();
		}
	}
}