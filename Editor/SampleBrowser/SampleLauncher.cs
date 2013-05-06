using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

		public SampleLauncher(Type resolver)
		{
			this.resolver = resolver;
		}

		private void LoadOpenTKResolverAssembly()
		{
			string pathToOpenTKResolver = GetOpenTKResolverPath();
			Assembly assembly = Assembly.LoadFrom(pathToOpenTKResolver);
			foreach (var type in assembly.GetTypes().Where(type => type.Name == "OpenTKResolver"))
				resolver = type;
		}

		private static string GetOpenTKResolverPath()
		{
			return
				Path.GetFullPath(Path.Combine("..", "..", "..", "Platforms", "WindowsOpenTK", "bin",
					"Debug", "DeltaEngine.WindowsOpenTK.dll"));
		}

		private Type resolver;

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
			return File.Exists(sample.AssemblyFilePath);
		}
	}
}