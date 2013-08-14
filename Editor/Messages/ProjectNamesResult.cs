namespace DeltaEngine.Editor.Messages
{
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