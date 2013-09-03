using System;

namespace DeltaEngine.Editor.AppBuilder
{
	internal class DemoBuiltAppsListForDesigner : BuiltAppsListViewModel
	{
		public DemoBuiltAppsListForDesigner()
		{
			AddApp(new WindowsAppInfo("Rebuildable app", Guid.NewGuid(), DateTime.Now)
			{
				SolutionFilePath = "A.sln"
			});
			AddApp(new WindowsAppInfo("Non-Rebuildable app ", Guid.NewGuid(), DateTime.Now));
		}
	}
}