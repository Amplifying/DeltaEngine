namespace DeltaEngine.Editor.Messages
{
	public class CreateProject
	{
		protected CreateProject() {}

		public CreateProject(string projectName, string starterKit)
		{
			ProjectName = projectName;
			StarterKit = starterKit;
		}

		public string ProjectName { get; private set; }
		public string StarterKit { get; private set; }
	}
}
