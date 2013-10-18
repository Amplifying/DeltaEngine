using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.ProjectCreator
{
	/// <summary>
	/// Required information for creation of a Delta Engine C# project.
	/// </summary>
	public class CsProject
	{
		public CsProject()
		{
			Name = "NewDeltaEngineProject";
			Framework = DeltaEngineFramework.OpenTK;
			Path = PathExtensions.GetVisualStudioProjectsFolder();
		}

		public string Name { get; set; }
		public DeltaEngineFramework Framework { get; set; }
		public string Path { get; set; }
	}
}