using DeltaEngine.Core;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace Breakout
{
	/// <summary>
	/// Just renders the background graphic
	/// </summary>
	public class Background : Sprite
	{
		public Background(Content content, Renderer renderer)
			: base(content.Load<Image>("Background"), renderer.Screen.Viewport)
		{
			renderer.Screen.ViewportSizeChanged += () => DrawArea = renderer.Screen.Viewport;
		}
	}
}