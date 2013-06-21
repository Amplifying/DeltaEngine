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
		public Background(ContentLoader content)
			: base(content.Load<Image>("Background"), Rectangle.One)
		{
			RenderLayer = DefaultRenderLayer;
		}
	}
}