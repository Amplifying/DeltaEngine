namespace DeltaEngine.Editor.Messages
{
	public class DeleteProject
	{
		public DeleteProject(string projectName)
		{
			ProjectName = projectName;
		}

		protected string ProjectName { get; set; }
	}
}