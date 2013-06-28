using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Profiling;
using DeltaEngine.Rendering;

namespace DeltaEngine.Scenes.UserInterfaces.Graphing
{
	/// <summary>
	/// Draws an overlay showing code profiling information - eg. how long Rendering took to run
	/// </summary>
	public abstract class CodeProfilingGraph : Graph
	{
		protected CodeProfilingGraph(ProfilingMode profilingMode)
		{
			this.profilingMode = profilingMode;
			RenderLayer = int.MaxValue - 10;
			Visibility = Visibility.Hide;
			NumberOfPercentiles = 5;
			MaximumNumberOfPoints = 100;
			PercentileSuffix = "%";
			CodeProfiler.Current.IsActive = true;
			CodeProfiler.Current.Updated += Update;
			Start<ToggleVisibility>();
		}

		private readonly ProfilingMode profilingMode;

		private void Update()
		{
			if (Visibility == Visibility.Hide)
				return;

			CodeProfilingResults results = CodeProfiler.Current.GetProfilingResults(profilingMode);
			UpdateLinesAndColors(results);
			AddValuesToLines(results);
		}

		private void UpdateLinesAndColors(CodeProfilingResults results)
		{
			for (int index = 0; index < Lines.Count; index++)
				Lines[index].Color = Color.GetHeatmapColor(index / (results.Sections.Count - 1.0f));

			for (int index = Lines.Count; index < results.Sections.Count; index++)
				CreateLine(GetKey(results.Sections[index].Name),
					Color.GetHeatmapColor(index / (results.Sections.Count - 1.0f)));
		}

		private static string GetKey(string name)
		{
			if (name.Contains("+"))
				return name.Substring(name.IndexOf('+') + 1);

			return name.Contains(".") ? name.Substring(name.LastIndexOf('.') + 1) : name;
		}

		private void AddValuesToLines(CodeProfilingResults results)
		{
			for (int index = 0; index < results.Sections.Count; index++)
				Lines[index].AddValue(GetSectionPercentage(results, index));
		}

		private static float GetSectionPercentage(CodeProfilingResults results, int index)
		{
			return 100.0f * results.Sections[index].TotalTime / results.TotalSectionTime;
		}

		public class ToggleVisibility : EventListener2D
		{
			public ToggleVisibility(InputCommands input)
			{
				input.Add(Key.F3, key => ToggleVisibilityForAllGraphs());
			}

			private void ToggleVisibilityForAllGraphs()
			{
				var graphs = EntitySystem.Current.GetEntitiesByHandler(this);
				foreach (Entity graph in graphs)
					graph.Set(GetToggledVisibility(graph.Get<Visibility>()));
			}

			private static Visibility GetToggledVisibility(Visibility currentVisibility)
			{
				return currentVisibility == Visibility.Show ? Visibility.Hide : Visibility.Show;
			}

			public override void ReceiveMessage(Entity2D entity, object message) {}
		}
	}
}