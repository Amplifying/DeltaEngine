using System;

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
			Location = GetEnvironmentVariableWithFallback();
		}

		public string Name { get; set; }
		public DeltaEngineFramework Framework { get; set; }
		public string Location { get; set; }

		public static string GetEnvironmentVariableWithFallback()
		{
			const string EnvironmentVariable = "%DeltaEnginePath%\\";
			string path = Environment.ExpandEnvironmentVariables(EnvironmentVariable);
			return EnvironmentVariable == path ? "C:\\Code\\DeltaEngine\\Samples\\" : path;
		}
	}
}