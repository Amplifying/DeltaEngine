using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Searches all Samples and passes them into a container.
	/// </summary>
	public class SampleCreator
	{
		public SampleCreator()
			: this(new FileSystem()) {}

		public SampleCreator(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
			Samples = new List<Sample>();
			solutionSamplesPath = GetSolutionPath(Directory.GetCurrentDirectory());
			solutionTutorialsPath = Path.Combine(solutionSamplesPath, "..", "Tutorials");
			InstallPath = PathExtensions.GetDeltaEngineInstalledDirectory() ??
				GetParentOfWorkingDirectory();
		}

		private readonly IFileSystem fileSystem;
		public List<Sample> Samples { get; private set; }
		private readonly string solutionSamplesPath;
		private readonly string solutionTutorialsPath;

		private static string GetSolutionPath(string subDirectory)
		{
			string parentDirectory = Path.GetFullPath(Path.Combine(subDirectory, ".."));
			if (Path.GetFileName(parentDirectory) == "DeltaEngine")
				return Path.Combine(parentDirectory, "Samples");
			if (Path.GetPathRoot(parentDirectory) == parentDirectory)
				return "";
			return GetSolutionPath(parentDirectory);
		}

		public string InstallPath { get; set; }

		private static string GetParentOfWorkingDirectory()
		{
			return new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName;
		}

		public void CreateSamples(DeltaEngineFramework framework)
		{
			if (fileSystem.Directory.Exists(solutionSamplesPath) &&
				fileSystem.Directory.Exists(solutionTutorialsPath))
			{
				GetSamplesFromSolutionDirectory();
				GetTutorialsFromSolutionDirectory();
			}
			else if (fileSystem.Directory.Exists(InstallPath))
			{
				GetExecutablesFromInstallPath(framework, SampleCategory.Game);
				GetExecutablesFromInstallPath(framework, SampleCategory.Tutorial);
			}
		}

		private void GetExecutablesFromInstallPath(DeltaEngineFramework framework,
			SampleCategory category)
		{
			if (category != SampleCategory.Game && category != SampleCategory.Tutorial)
				return;
			string[] directories =
				fileSystem.Directory.GetDirectories(Path.Combine(InstallPath, framework.ToString(),
					category == SampleCategory.Game ? "Samples" : "Tutorials"));
			foreach (string projectDirectory in directories)
			{
				if (projectDirectory.Contains("EmptyLibrary"))
					continue;
				string projectName = GetProjectNameFromLocation(projectDirectory);
				string prefix = category == SampleCategory.Game ? "" : "DeltaEngine.Tutorials.";
				string projectFile = Path.Combine(projectDirectory, prefix + projectName + ".csproj");
				string executableFile = Path.Combine(InstallPath, framework.ToString(),
					prefix + projectName + ".exe");
				Samples.Add(category == SampleCategory.Game
					? Sample.CreateGame(projectName, projectFile, executableFile)
					: Sample.CreateTutorial(projectName, projectFile, executableFile));
			}
		}

		private static string GetProjectNameFromLocation(string projectDirectory)
		{
			string name = projectDirectory.TrimEnd(Path.DirectorySeparatorChar);
			return name.Split(Path.DirectorySeparatorChar).Last();
		}

		private void GetSamplesFromSolutionDirectory()
		{
			string[] directories = fileSystem.Directory.GetDirectories(solutionSamplesPath);
			foreach (string projectDirectory in directories)
			{
				string projectName = GetProjectNameFromLocation(projectDirectory);
				if (!HasExecutableFile(projectDirectory, projectName))
					continue;
				string projectFile = Path.Combine(projectDirectory, projectName + ".csproj");
				if (!fileSystem.File.Exists(projectFile))
					continue;
				AddSampleGame(projectDirectory, projectName, projectFile);
				string pathToTests = Path.Combine(projectDirectory, "Tests", "bin", GetConfigurationName());
				if (!fileSystem.Directory.Exists(pathToTests))
					continue;
				AddVisualTests(pathToTests, projectName, projectFile);
			}
		}

		private void GetTutorialsFromSolutionDirectory()
		{
			string[] directories = fileSystem.Directory.GetDirectories(solutionTutorialsPath);
			foreach (string projectDirectory in directories)
			{
				string projectName = "DeltaEngine.Tutorials." + Path.GetFileName(projectDirectory);
				if (!HasExecutableFile(projectDirectory, projectName))
					continue;
				string projectFile = Path.Combine(projectDirectory, projectName + ".csproj");
				if (!fileSystem.File.Exists(projectFile))
					continue;
				AddTutorial(projectDirectory, projectName, projectFile);
			}
		}

		private bool HasExecutableFile(string projectDirectory, string projectName)
		{
			string exePath = Path.Combine(projectDirectory, "bin", GetConfigurationName(),
				projectName + ".exe");
			return fileSystem.File.Exists(exePath);
		}

		private static string GetConfigurationName()
		{
			return ExceptionExtensions.IsDebugMode ? "Debug" : "Release";
		}

		private void AddSampleGame(string projectDirectory, string projectName, string projectFile)
		{
			string executableFile = Path.Combine(projectDirectory, "bin", GetConfigurationName(),
				projectName + ".exe");
			Samples.Add(Sample.CreateGame(projectName, projectFile, executableFile));
		}

		private void AddVisualTests(string pathToTestDirectory, string projectName,
			string pathToProjectFile)
		{
			foreach (var file in fileSystem.Directory.GetFiles(pathToTestDirectory))
			{
				if (!file.EndsWith(projectName + ".Tests.exe") &&
					!file.EndsWith(projectName + ".Tests.dll"))
					continue;
				try
				{
					Assembly assembly = Assembly.LoadFrom(file);
					foreach (var type in assembly.GetTypes())
					{
						if (type.IsDefined(typeof(CompilerGeneratedAttribute), false) || !IsVisualTestClass(type))
							continue;
						foreach (var method in type.GetMethods().Where(IsVisualTestMethod))
							Samples.Add(Sample.CreateTest(projectName + ": " + method.Name, pathToProjectFile, file,
								type.Name, method.Name));
					}
				}
				catch (ReflectionTypeLoadException ex)
				{
					Logger.Warning("Failed to load " + file + ". LoaderExceptions: " +
						ex.LoaderExceptions.ToText());
				}
			}
		}

		private void AddTutorial(string projectDirectory, string projectName, string projectFile)
		{
			string executableFile = Path.Combine(projectDirectory, "bin", GetConfigurationName(),
				projectName + ".exe");
			Samples.Add(Sample.CreateTutorial(projectName, projectFile, executableFile));
		}

		private static bool IsVisualTestClass(Type type)
		{
			return type.BaseType.FullName == "DeltaEngine.Platforms.TestWithMocksOrVisually";
		}

		private static bool IsVisualTestMethod(MethodInfo method)
		{
			object[] attributes = method.GetCustomAttributes(false);
			bool isNUnitTest = false;
			bool isCloseAfterFirstFrame = false;
			foreach (object attribute in attributes)
			{
				if (attribute.GetType().FullName == "NUnit.Framework.TestAttribute")
					isNUnitTest = true;
				if (attribute.GetType().FullName == "DeltaEngine.Platforms.CloseAfterFirstFrameAttribute")
					isCloseAfterFirstFrame = true;
			}
			return isNUnitTest && !isCloseAfterFirstFrame;
		}

		private void GetSamplesFromFallbackDirectory()
		{
			if (!fileSystem.Directory.Exists(InstallPath))
				return;
			string[] files = fileSystem.Directory.GetFiles(InstallPath);
			foreach (string file in files)
			{
				if (!file.Contains(".exe") || file.Contains("Editor") || file.Contains("Uninstall"))
					continue;
				string name = file.Split(Path.DirectorySeparatorChar).Last();
				name = name.Split('.').First();
				Samples.Add(Sample.CreateGame(name, "", file));
			}
		}

		private void GetSamplesFromDeltaEngine(string directory)
		{
			if (!fileSystem.Directory.Exists(directory))
				return;
			string[] directories = fileSystem.Directory.GetDirectories(directory);
			foreach (string projectDirectory in directories)
			{
				if (excludedDirectories.Any(s => projectDirectory.Contains(s)))
					continue;
				GetSamplesFromDeltaEngine(projectDirectory);
				if (!projectDirectory.Contains("Tests"))
					continue;
				string projectFile = "";
				foreach (var file in
					fileSystem.Directory.GetFiles(projectDirectory).Where(
						file => Path.GetExtension(file) == ".csproj"))
				{
					projectFile = file;
					break;
				}
				if (!fileSystem.File.Exists(projectFile))
					continue;
				string projectName = Path.GetFileNameWithoutExtension(projectFile);
				projectName = projectName.Replace(".Tests", "");
				GetDeltaEngineTestsProjects(projectName, projectDirectory, projectFile);
			}
		}

		private readonly List<string> excludedDirectories = new List<string>
		{
			".",
			"Properties",
			"Editor",
			"packages",
			"Samples",
			"VisualStudioTemplates",
			"bin",
			"obj"
		};

		private void GetDeltaEngineTestsProjects(string projectName, string projectPath,
			string projectFile)
		{
			string pathToTests = Path.Combine(projectPath, "bin", GetConfigurationName());
			if (fileSystem.Directory.Exists(pathToTests))
				AddVisualTests(pathToTests, projectName, projectFile);
		}

		public bool IsSourceCodeRelease()
		{
			return fileSystem.Directory.Exists(solutionSamplesPath);
		}
	}
}