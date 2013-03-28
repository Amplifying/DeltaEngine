using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace DeltaEngine.Multimedia.SharpDX
{
	public class SharpDXVideoSurface : VideoSurface
	{
		public SharpDXVideoSurface(Video video, Drawing drawing, Renderer renderer)
			: base(drawing, renderer, video) {}
	}
}
