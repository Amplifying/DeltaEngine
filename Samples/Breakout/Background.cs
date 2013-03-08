using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace Breakout
{
	/// <summary>
	/// Just renders the background graphic
	/// </summary>
	public class Background : Sprite
	{
		public Background(Content content)
			: base(content.Load<Image>("Background"), Rectangle.One)
		{
			RenderLayer = MinRenderLayer;
		}
	}
}