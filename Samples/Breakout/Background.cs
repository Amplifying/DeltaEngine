using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;

namespace Breakout
{
	/// <summary>
	/// Just renders the background graphic
	/// </summary>
	public class Background : Sprite
	{
		public Background()
			: base(ContentLoader.Load<Image>("Background"), Rectangle.One)
		{
			RenderLayer = DefaultRenderLayer;
		}
	}
}