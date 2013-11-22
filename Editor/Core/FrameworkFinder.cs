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
					Default = DeltaEngineFramework.OpenTK;
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

		public DeltaEngineFramework[] All { get; private set; }
		public DeltaEngineFramework Default { get; private set; }
	}
}