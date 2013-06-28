using DeltaEngine.Core;

namespace DeltaEngine.Editor.Builder
{
	public class ProjectEntry
	{
		public ProjectEntry(string projectEntryString)
		{
			string[] dataParts = projectEntryString.SplitAndTrim("Project(", "\"", ") =", ",");
			TypeGuid = dataParts[0];
			Title = dataParts[1];
			FilePath = dataParts[2];
			Guid = dataParts[3];
		}

		public string TypeGuid { get; private set; }
		public string Title { get; private set; }
		public string FilePath { get; private set; }
		public string Guid { get; private set; }

		public bool IsCSharpProject
		{
			get { return TypeGuid == CSharpProjectTypeGuid; }
		}

		public const string CSharpProjectTypeGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

		public bool IsSolutionFolder
		{
			get { return TypeGuid == ProjectFolderGuid; }
		}

		public const string ProjectFolderGuid = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
	}
}