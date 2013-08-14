namespace DeltaEngine.Editor.Messages
{
	/// <summary>
	/// Request to change project from the client to the server. Server sends back SetProject
	/// </summary>
	public class ChangeProjectRequest
	{
		protected ChangeProjectRequest() {}

		public ChangeProjectRequest(string projectName)
		{
			ProjectName = projectName;
		}

		public string ProjectName { get; private set; }
	}
}