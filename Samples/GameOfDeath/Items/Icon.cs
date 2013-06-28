using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Icons for the items are shown at the top of the screen.
	/// </summary>
	public class Icon : Sprite
	{
		public Icon(int iconIndex, ScreenSpace screen)
			: base(
				ContentLoader.Load<Image>(IconNames[iconIndex]), GetIconDrawArea(iconIndex, screen.Viewport))
		{
			RenderLayer = (int)GameCoordinator.RenderLayers.Icons;
			Color = Color.Gray;
			screen.ViewportSizeChanged += () => DrawArea = GetIconDrawArea(iconIndex, screen.Viewport);
		}

		private static readonly string[] IconNames =
		{
			"IconMallet", "IconFire", "IconToxic",
			"IconAtomic"
		};

		private static Rectangle GetIconDrawArea(int index, Rectangle viewport)
		{
			return Rectangle.FromCenter(0.39f + index * 0.08f, viewport.Top + 0.04f, 0.07f, 0.08f);
		}
	}
}