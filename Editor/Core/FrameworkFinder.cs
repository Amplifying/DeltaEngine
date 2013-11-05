using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.Core
{
	public class FrameworkFinder
	{
		//ncrunch: no coverage start
		public FrameworkFinder()
		{
			var installDirectory = GetInstallPath();
			var installedFrameworks = new List<DeltaEngineFramework>();
			foreach (var directoryInfo in GetInstallerDirectoryInfo(installDirectory).GetDirectories())
			{
				var framework = DeltaEngineFrameworkExtensions.FromString(directoryInfo.Name);
				if (framework == DeltaEngineFramework.Default || !IsValid(directoryInfo))
					continue;
				installedFrameworks.Add(framework);
				if (directoryInfo.Name == "OpenTK")
					editorFramework = DeltaEngineFramework.OpenTK;
			}
			All = installedFrameworks.ToArray();
		}

		private static string GetInstallPath()
		{
			var installDirectory = PathExtensions.GetDeltaEngineInstalledDirectory();
			if (installDirectory != null)
				return installDirectory;
			Logger.Warning("Environment Variable '" + PathExtensions.EnginePathEnvironmentVariableName +
				"' not set. Please use the DeltaEngine installer to set it up.");
			return "";
		}

		private static DirectoryInfo GetInstallerDirectoryInfo(string installDirectory)
		{
			var workingDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
			return String.IsNullOrEmpty(installDirectory)
				? IsFrameworkDirectory(workingDirectory) ? workingDirectory.Parent : workingDirectory
				: new DirectoryInfo(installDirectory);
		}

		public static bool IsFrameworkDirectory(DirectoryInfo directory)
		{
			foreach (DeltaEngineFramework framework in Enum.GetValues(typeof(DeltaEngineFramework)))
				if (framework.ToString() == directory.Name)
					return true;
			return false;
		}

		private static bool IsValid(DirectoryInfo directoryInfo)
		{
			int numberOfProperFolders =
				directoryInfo.GetDirectories().Count(
					directory => directory.Name == "Samples" || directory.Name == "VisualStudioTemplates");
			return numberOfProperFolders == 2;
		}

		private readonly DeltaEngineFramework editorFramework;

		public DeltaEngineFramework[] All { get; private set; }

		public DeltaEngineFramework Default
		{
			get
			{
				if (editorFramework == DeltaEngineFramework.Default)
					throw new EditorDefaultFrameworkNotInstalled();
				return editorFramework;
			}
		}

		public class EditorDefaultFrameworkNotInstalled : Exception {}

		public DeltaEngineFramework Current
		{
			get
			{
				if (current == DeltaEngineFramework.Default)
					current = Default;
				return current;
			}
			set { current = value; }
		}

		private DeltaEngineFramework current;

		public string SamplesPath
		{
			get { return Path.Combine(Current.ToString(), "Samples"); }
		}

		public string TemplatesPath
		{
			get { return Path.Combine(Current.ToString(), "VisualStudioTemplates", "Delta Engine"); }
		}
	}
}