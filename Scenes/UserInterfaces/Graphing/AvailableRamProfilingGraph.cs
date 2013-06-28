using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace DeltaEngine.Scenes.UserInterfaces.Graphing
{
	public class AvailableRamProfilingGraph : SystemProfilingGraph
	{
		public AvailableRamProfilingGraph()
			: base(ProfilingMode.AvailableRAM, Color.Green)
		{
			DrawArea = Rectangle.FromCenter(0.6f, 0.5f, 0.3f, 0.2f);
			Viewport = new Rectangle(0.0f, 0.0f, 100.0f, 1.0f);
			Start<AutogrowViewport>();
		}
	}
}