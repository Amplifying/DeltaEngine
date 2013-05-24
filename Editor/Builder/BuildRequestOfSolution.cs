using System.IO;
using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Builder
{
	public class BuildRequestOfSolution : BuildRequest
	{
		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction
		/// </summary>
		protected BuildRequestOfSolution() {}

		public BuildRequestOfSolution(string codeSolutionFilePath, PlatformName platform)
		{
			CodeSolutionFilePath = codeSolutionFilePath;
			Platform = platform;
			LoadSolutionData();
		}

		public string CodeSolutionFilePath { get; private set; }

		private void LoadSolutionData()
		{
			DetermineProjectName();
			LoadAndSerializeProjectCode();
		}

		private void DetermineProjectName()
		{
			ProjectName = Path.GetFileNameWithoutExtension(CodeSolutionFilePath);
		}

		protected virtual void LoadAndSerializeProjectCode()
		{
			string solutionDirectory = Path.GetDirectoryName(CodeSolutionFilePath);
			SerializedProjectData = new CodeData(solutionDirectory).GetBytes();
		}
	}
}