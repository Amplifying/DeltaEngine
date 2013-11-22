namespace DeltaEngine.Editor.Messages
{
	/// <summary>
	/// Names of accessible projects for the currently logged in user
	/// </summary>
	public class ProjectNamesResult
	{
		protected ProjectNamesResult() {}

		public ProjectNamesResult(string[] projectNames)
		{
			ProjectNames = projectNames;
		}

		public string[] ProjectNames { get; private set; }
	}
}