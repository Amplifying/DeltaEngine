namespace DeltaEngine.Editor.Messages
{
	public class CreateProject
	{
		protected CreateProject() {}

		public CreateProject(string projectName)
		{
			ProjectName = projectName;
		}

		public string ProjectName { get; private set; }
	}
}
