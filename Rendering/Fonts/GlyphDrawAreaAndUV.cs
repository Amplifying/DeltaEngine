using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Fonts
{
	/// <summary>
	/// Glyph draw info is used by FontData and for rendering glpyhs on the screen.
	/// </summary>
	public struct GlyphDrawAreaAndUV
	{
		public Rectangle DrawArea;
		public Rectangle UV;
	}
}