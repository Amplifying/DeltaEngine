using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Starts files associated with Samples.
	/// </summary>
	public class SampleLauncher
	{
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

		private static void StartTest(string assembly, string entryClass, string entryMethod)
		{
			using (var starter = new AssemblyStarter(assembly, false))
				starter.Start(entryClass, entryMethod);
		}

		public bool DoesAssemblyExist(Sample sample)
		{
			return File.Exists(sample.AssemblyFilePath);
		}
	}
}