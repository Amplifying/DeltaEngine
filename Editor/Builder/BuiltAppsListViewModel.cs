using System.Collections.Generic;

namespace DeltaEngine.Editor.Builder
{
	public class BuiltAppsListViewModel
	{
		public BuiltAppsListViewModel()
		{
			BuiltApps = new List<BuiltApp>();
		}

		public List<BuiltApp> BuiltApps { get; set; }
	}
}