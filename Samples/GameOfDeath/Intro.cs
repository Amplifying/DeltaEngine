using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace GameOfDeath
{
	/// <summary>
	/// For the first few seconds of the application start a big logo with credits is displayed.
	/// </summary>
	public class Intro
	{
		public Intro(ContentLoader content)
		{
			var image = content.Load<Image>("Logo");
			var drawArea = Rectangle.FromCenter(Point.Half,
				image.PixelSize / Scoreboard.QuadraticFullscreenSize);
			CreateLogo(image, drawArea);
		}

		private static void CreateLogo(Image image, Rectangle drawArea)
		{
			var logo = new Sprite(image, drawArea)
			{
				RenderLayer = (int)GameCoordinator.RenderLayers.IntroLogo
			};
			FadeOut(logo);
		}

		private static void FadeOut(Entity2D entity)
		{
			entity.Add<FinalTransition>().Add(new Transition.Duration(5.0f)).Add(
				new Transition.FadingColor(entity.Color));
		}
	}
}