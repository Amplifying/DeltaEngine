using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace GameOfDeath
{
	/// <summary>
	/// For the first few seconds of the application start a big logo with credits is displayed.
	/// </summary>
	public class Intro : FadeoutEffect
	{
		public Intro(Content content)
			: base(content.Load<Image>("Logo"), Rectangle.Zero, 5)
		{
			DrawArea = Rectangle.FromCenter(Point.Half, image.PixelSize / Score.QuadraticFullscreenSize);
			RenderLayer = 255;
		}

		protected override void Render(Renderer renderer, Time time)
		{
			renderer.DrawRectangle(Rectangle.One, new Color(0, 0, 0, Color.AlphaValue));
			base.Render(renderer, time);
		}
	}
}