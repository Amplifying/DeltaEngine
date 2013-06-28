using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace DeltaEngine.Scenes.UserInterfaces.Graphing
{
	public class FpsProfilingGraph : SystemProfilingGraph
	{
		public FpsProfilingGraph()
			: base(ProfilingMode.Fps, Color.Yellow)
		{
			DrawArea = Rectangle.FromCenter(0.2f, 0.5f, 0.3f, 0.2f);
			Viewport = new Rectangle(0.0f, 0.0f, 100.0f, 60.0f);
		}
	}
}