using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DeltaEngine.Core;

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
			SamplesPath =
				Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Samples"));
			DeltaEngineRootPath =
				Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ""));
			FallbackPath =
				Path.GetFullPath(Environment.ExpandEnvironmentVariables("%DeltaEnginePath%\\"));
		}

		private readonly IFileSystem fileSystem;

		public List<Sample> Samples { get; private set; }

		public string SamplesPath { get; set; }
		public string DeltaEngineRootPath { get; set; }
		public string FallbackPath { get; set; }

		public void CreateSamples()
		{
			if (fileSystem.Directory.Exists(SamplesPath))
				GetSamplesFromSamplesDirectory();
			else
				GetSamplesFromFallbackDirectory();

			GetSamplesFromDeltaEngine(DeltaEngineRootPath);
		}

		private void GetSamplesFromSamplesDirectory()
		{
			string[] directories = fileSystem.Directory.GetDirectories(SamplesPath);
			foreach (string projectDirectory in directories)
			{
				string projectName = GetProjectNameFromLocation(projectDirectory);
				if (!HasExecutableFile(projectDirectory, projectName))
					continue;

				string projectFile = Path.Combine(projectDirectory, projectName + ".csproj");
				if (!fileSystem.File.Exists(projectFile))
					continue;

				AddSampleGame(projectDirectory, projectName, projectFile);
				string pathToTests = Path.Combine(projectDirectory, "Tests", "bin", "Debug");
				if (!fileSystem.Directory.Exists(pathToTests))
					continue;

				AddVisualTests(pathToTests, projectName, projectFile);
			}
		}

		private static string GetProjectNameFromLocation(string projectDirectory)
		{
			string name = projectDirectory.TrimEnd(Path.DirectorySeparatorChar);
			return name.Split(Path.DirectorySeparatorChar).Last();
		}

		private bool HasExecutableFile(string projectDirectory, string projectName)
		{
			string exePath = Path.Combine(projectDirectory, "bin", "Debug", projectName + ".exe");
			return fileSystem.File.Exists(exePath);
		}

		private void AddSampleGame(string projectDirectory, string projectName, string projectFile)
		{
			string executableFile = Path.Combine(projectDirectory, "bin", "Debug", projectName + ".exe");
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
						if (type.IsDefined(typeof(CompilerGeneratedAttribute), false))
							continue;

						foreach (var method in type.GetMethods().Where(IsVisualTest))
							Samples.Add(Sample.CreateTest(projectName + ": " + method.Name, pathToProjectFile, file,
								type.Name, method.Name));
					}
				}
				catch (ReflectionTypeLoadException ex)
				{
					Console.WriteLine("Failed to load " + file + ". LoaderExceptions: " +
						ex.LoaderExceptions.ToText());
				}
			}
		}

		private static bool IsVisualTest(MethodInfo method)
		{
			object[] attributes = method.GetCustomAttributes(false);
			return attributes.Any(attribute => attribute.GetType().ToString() == VisualTestAttribute);
		}

		private const string VisualTestAttribute = "DeltaEngine.Platforms.All.VisualTestAttribute";

		private void GetSamplesFromFallbackDirectory()
		{
			if (!fileSystem.Directory.Exists(FallbackPath))
				return;
			string[] files = fileSystem.Directory.GetFiles(FallbackPath);
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
			string pathToTests = Path.Combine(projectPath, "bin", "Debug");
			if (fileSystem.Directory.Exists(pathToTests))
				AddVisualTests(pathToTests, projectName, projectFile);
		}
	}
}