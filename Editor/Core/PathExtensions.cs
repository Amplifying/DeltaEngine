using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.Core
{
	public static class PathExtensions
	{
		public static bool IsDeltaEnginePathEnvironmentVariableAvailable()
		{
			return !String.IsNullOrEmpty(GetDeltaEngineInstalledDirectory());
		}

		public static string GetDeltaEngineInstalledDirectory()
		{
			return Environment.GetEnvironmentVariable(EnginePathEnvironmentVariableName);
		}

		public const string EnginePathEnvironmentVariableName = "DeltaEnginePath";

		//ncrunch: no coverage start
		public static string GetFallbackEngineSourceCodeDirectory()
		{
			if (Directory.Exists(DefaultCodePath))
				return DefaultCodePath;
			const string DefaultDevelopmentPath = @"C:\Development\DeltaEngine";
			if (Directory.Exists(DefaultDevelopmentPath))
				return DefaultDevelopmentPath;
			throw new NoDeltaEngineFoundInFallbackPaths();
		} //ncrunch: no coverage end

		public const string DefaultCodePath = @"C:\Code\DeltaEngine";

		public class NoDeltaEngineFoundInFallbackPaths : Exception {}

		public static string GetEngineSolutionFilePath()
		{
			const string DeltaEngineSolutionFile = "DeltaEngine.sln";
			var solutionFilePath = GetFilePathFromSourceCode(DeltaEngineSolutionFile);
			if (String.IsNullOrEmpty(solutionFilePath))
				throw new FileNotFoundException(DeltaEngineSolutionFile); //ncrunch: no coverage
			return solutionFilePath;
		}

		public static string GetFilePathFromSourceCode(string filename)
		{
			string originalDirectory = Environment.CurrentDirectory;
			try
			{
				if (StackTraceExtensions.StartedFromNCrunch)
					Environment.CurrentDirectory = GetFallbackEngineSourceCodeDirectory();
				return GetFilePathFromSourceCodeRecursively(GetWorkingDirectoryInfo(), filename);
			}
			finally
			{
				Environment.CurrentDirectory = originalDirectory;
			}
		}

		private static DirectoryInfo GetWorkingDirectoryInfo()
		{
			return new DirectoryInfo(Environment.CurrentDirectory);
		}

		private static string GetFilePathFromSourceCodeRecursively(DirectoryInfo directory,
			string filename)
		{
			if (directory.Parent == null)
				return null; //ncrunch: no coverage
			foreach (var file in directory.GetFiles())
				if (file.Name == filename)
					return file.FullName;
			return GetFilePathFromSourceCodeRecursively(directory.Parent, filename);
		}

		public static string GetSamplesSolutionFilePath()
		{
			const string SamplesSolutionFile = "DeltaEngine.Samples.sln";
			return GetFilePathFromSourceCode(SamplesSolutionFile) ??
				GetFilePathFromInstallerRelease(SamplesSolutionFile);
		}

		public static string GetFilePathFromInstallerRelease(string filePath)
		{
			return IsDeltaEnginePathEnvironmentVariableAvailable()
				? Path.Combine(GetDeltaEngineInstalledDirectory(), Path.Combine("OpenTK", filePath)) : null;
		}

		public static string GetInstalledOrFallbackEnginePath()
		{
			return IsDeltaEnginePathEnvironmentVariableAvailable()
				? GetDeltaEngineInstalledDirectory() : GetFallbackEngineSourceCodeDirectory();
		}
		
		public static string GetVisualStudioProjectsFolder()
		{
			foreach (var visualStudioProjectPath in GetSupportedVisualStudioProjectFolders())
				if (Directory.Exists(visualStudioProjectPath))
					return visualStudioProjectPath;
			throw new VisualStudioDefaultProjectsLocationNotFound(); //ncrunch: no coverage
		}

		private static IEnumerable<string> GetSupportedVisualStudioProjectFolders()
		{
			string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			const string ProjectsFolder = "Projects";
			return new[]
			{
				Path.Combine(myDocumentsPath, "Visual Studio 2013", ProjectsFolder),
				Path.Combine(myDocumentsPath, "Visual Studio 2012", ProjectsFolder),
				Path.Combine(myDocumentsPath, "Visual Studio 2010", ProjectsFolder),
				Path.Combine(myDocumentsPath, "Visual Studio 2008", ProjectsFolder)
			};
		}

		private class VisualStudioDefaultProjectsLocationNotFound : Exception {}
	}
}