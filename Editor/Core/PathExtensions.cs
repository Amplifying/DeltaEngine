using System;
using System.IO;

namespace DeltaEngine.Editor.Core
{
	public class PathExtensions
	{
		public static string GetDeltaEngineDirectory()
		{
			return Environment.GetEnvironmentVariable(EnginePathEnvironmentVariableName);
		}

		public const string EnginePathEnvironmentVariableName = "DeltaEnginePath";

		public static string GetEngineSolutionFilePath()
		{
			return Path.Combine(GetDeltaEngineDirectory(), "DeltaEngine.sln");
		}

		public static string GetSamplesSolutionFilePath()
		{
			return Path.Combine(GetDeltaEngineDirectory(), "DeltaEngine.Samples.sln");
		}
	}
}