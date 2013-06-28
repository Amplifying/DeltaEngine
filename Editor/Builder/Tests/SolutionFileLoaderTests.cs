using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class SolutionFileLoaderTests
	{
		[Test]
		public void LoadProjectEntriesOfDeltaEngineSolution()
		{
			var solutionLoader = new SolutionFileLoader(GetDeltaEngineSolutionFilePath());
			Assert.IsNotEmpty(solutionLoader.ProjectEntries);
		}

		private static string GetDeltaEngineSolutionFilePath()
		{
			string engineSolutionFile = Path.Combine(GetDeltaEngineRootDirectory(), "DeltaEngine.sln");
			Assert.IsTrue(File.Exists(engineSolutionFile));

			return engineSolutionFile;
		}

		private static string GetDeltaEngineRootDirectory()
		{
			return Environment.GetEnvironmentVariable("DeltaEnginePath");
		}

		[Test]
		public void GetCSharpProjectsOfDeltaEngineSolution()
		{
			var solutionLoader = new SolutionFileLoader(GetDeltaEngineSolutionFilePath());
			List<ProjectEntry> csharpProjects = solutionLoader.GetCSharpProjects();
			Assert.IsNotEmpty(csharpProjects);
			Assert.IsTrue(csharpProjects.Exists(project => project.Title == "DeltaEngine.Datatypes"));
		}

		[Test]
		public void GetProjectsFoldersOfDeltaEngineSolution()
		{
			var solutionLoader = new SolutionFileLoader(GetDeltaEngineSolutionFilePath());
			List<ProjectEntry> csharpProjects = solutionLoader.GetSolutionFolders();
			Assert.IsNotEmpty(csharpProjects);
			Assert.IsTrue(csharpProjects.Exists(project => project.Title == "Datatypes"));
		}

		[Test]
		public void GetSpecificCSharpProjectFromDeltaEngineSamplesSolution()
		{
			string engineSamplesSolution = Path.Combine(GetDeltaEngineRootDirectory(),
				"DeltaEngine.Samples.sln");
			Assert.IsTrue(File.Exists(engineSamplesSolution));

			var solutionLoader = new SolutionFileLoader(engineSamplesSolution);
			ProjectEntry logoAppProject = solutionLoader.GetCSharpProject("LogoApp");
			Assert.IsNotNull(logoAppProject);
			Assert.AreEqual("LogoApp", logoAppProject.Title);
		}
	}
}
