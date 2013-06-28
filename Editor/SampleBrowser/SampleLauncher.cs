using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using DeltaEngine.Core;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Starts files associated with Samples.
	/// </summary>
	public class SampleLauncher
	{
		public SampleLauncher()
		{
			LoadOpenTKResolverAssembly();
		}

		private void LoadOpenTKResolverAssembly()
		{
			string pathToOpenTKResolver = GetOpenTKResolverPath(Directory.GetCurrentDirectory());
			try
			{
				Assembly assembly = Assembly.LoadFrom(pathToOpenTKResolver);
				foreach (var type in assembly.GetTypes().Where(type => type.Name == "OpenTKResolver"))
					resolver = type;
			}
			catch (FileNotFoundException e)
			{
				resolver = null;
				MessageBox.Show(pathToOpenTKResolver + " not found: " + e.Message, "File not found");
			}
			catch (ReflectionTypeLoadException e)
			{
				resolver = null;
				MessageBox.Show("Failed to load OpenTK Resolver: " + e.LoaderExceptions.ToText(),
					"File not loaded");
			}
		}

		private Type resolver;

		public SampleLauncher(Type resolver)
		{
			this.resolver = resolver;
		}

		private string GetOpenTKResolverPath(string currentDirectory)
		{
			string parentDirectory = Path.GetFullPath(Path.Combine(currentDirectory, ".."));
			if (IsDeltaEngineDirectory(parentDirectory))
				return Path.GetFullPath(Path.Combine(parentDirectory, relativeResolverPath));

			return parentDirectory != Path.GetPathRoot(parentDirectory)
				? GetOpenTKResolverPath(parentDirectory) : "";
		}

		private static bool IsDeltaEngineDirectory(string path)
		{
			return
				Directory.GetDirectories(path).Any(
					directory => Path.GetFileName(directory.TrimEnd(Path.DirectorySeparatorChar)) == "Editor");
		}

		private readonly string relativeResolverPath = Path.Combine("Platforms", "WindowsOpenTK",
			"bin", "Debug", "DeltaEngine.WindowsOpenTK.dll");

		public void OpenProject(Sample sample)
		{
			OpenFile(sample.ProjectFilePath);
		}

		private static void OpenFile(string filePath)
		{
			int index = filePath.LastIndexOf(@"\", StringComparison.Ordinal);
			string exeDirectory = filePath.Substring(0, index);
			var compiledOutputDirectory = new ProcessStartInfo(filePath)
			{
				WorkingDirectory = exeDirectory
			};
			Process.Start(compiledOutputDirectory);
		}

		public bool DoesProjectExist(Sample sample)
		{
			return File.Exists(sample.ProjectFilePath);
		}

		public void StartExecutable(Sample sample)
		{
			if (sample.Category == SampleCategory.Test)
				StartTest(sample.AssemblyFilePath, sample.EntryClass, sample.EntryMethod);
			else
				OpenFile(sample.AssemblyFilePath);
		}

		private void StartTest(string assembly, string entryClass, string entryMethod)
		{
			using (var starter = new AssemblyStarter(assembly))
				starter.Start(entryClass, entryMethod, new object[] { resolver });
		}

		public bool DoesAssemblyExist(Sample sample)
		{
			if (sample.Category == SampleCategory.Test)
				return File.Exists(sample.AssemblyFilePath) && resolver != null;

			return File.Exists(sample.AssemblyFilePath);
		}
	}
}