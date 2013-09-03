using System;
using System.IO;

namespace DeltaEngine.Editor.Messages
{
	public class AppBuildRequest : BuildMessage
	{
		/// <summary>
		/// Need empty constructor for serialization and reconstruction.
		/// </summary>
		protected AppBuildRequest() {}

		public AppBuildRequest(string solutionFileName, string projectName, PlatformName platform,
			byte[] serializedCodeData)
		{
			SolutionFileName = solutionFileName;
			ProjectName = projectName;
			Platform = platform;
			PackedCodeData = serializedCodeData;
			ValidateData();
		}

		public string SolutionFileName { get; private set; }
		public string ProjectName { get; private set; }
		public PlatformName Platform { get; private set; }
		public byte[] PackedCodeData { get; private set; }

		private void ValidateData()
		{
			if (SolutionFileName == null || !Path.HasExtension(SolutionFileName))
				throw new NoSolutionFileNameSpecified();

			if (String.IsNullOrEmpty(ProjectName))
				throw new NoProjectNameSpecified();

			if (PackedCodeData == null || PackedCodeData.Length == 0)
				throw new NoPackedCodeDataSpecified();
		}

		public class NoSolutionFileNameSpecified : Exception { }
		public class NoProjectNameSpecified : Exception { }
		public class NoPackedCodeDataSpecified : Exception { }
	}
}