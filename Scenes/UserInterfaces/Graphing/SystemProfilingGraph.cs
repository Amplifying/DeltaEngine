using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Profiling;
using DeltaEngine.Rendering;

namespace DeltaEngine.Scenes.UserInterfaces.Graphing
{
	public abstract class SystemProfilingGraph : Graph
	{
		protected SystemProfilingGraph(ProfilingMode profilingMode, Color color)
		{
			Add(profilingMode);
			RenderLayer = int.MaxValue - 10;
			Visibility = Visibility.Hide;
			NumberOfPercentiles = 5;
			MaximumNumberOfPoints = 100;
			ArePercentileLabelsInteger = true;
			CreateLine(profilingMode.ToString(), color);
			Start<LogProfilingData, CodeProfilingGraph.ToggleVisibility>();
			SystemProfiler.Current.IsActive = true;
			SystemProfiler.Current.Updated += Update;
		}

		private class LogProfilingData : Behavior2D
		{
			public LogProfilingData(SystemInformation systemInformation)
			{
				this.systemInformation = systemInformation;
			}

			private readonly SystemInformation systemInformation;

			public override void Handle(Entity2D entity)
			{
				SystemProfiler.Current.Log(ProfilingMode.Fps | ProfilingMode.AvailableRAM,
					systemInformation);
			}
		}

		private void Update()
		{
			if (Visibility == Visibility.Hide)
				return;

			SystemProfilerSection results =
				SystemProfiler.Current.GetProfilingResults(Get<ProfilingMode>());
			Lines[0].AddValue(results.TotalValue / results.Calls);
		}
	}
}