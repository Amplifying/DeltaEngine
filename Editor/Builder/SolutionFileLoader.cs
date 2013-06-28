using System;
using System.Collections.Generic;
using System.IO;

namespace DeltaEngine.Editor.Builder
{
	public class SolutionFileLoader
	{
		public SolutionFileLoader(string solutionFilePath)
		{
			solutionFile = solutionFilePath;
			solutionContentLines = File.ReadAllLines(solutionFile);
			LoadProjectEntries();
		}

		private readonly string solutionFile;
		private readonly string[] solutionContentLines;

		private void LoadProjectEntries()
		{
			ProjectEntries = new List<ProjectEntry>();
			const string ProjectIdentifier = "Project(";
			foreach (string contentLine in solutionContentLines)
				if (contentLine.StartsWith(ProjectIdentifier))
					ProjectEntries.Add(new ProjectEntry(contentLine));
		}

		public List<ProjectEntry> ProjectEntries { get; private set; }

		public List<ProjectEntry> GetCSharpProjects()
		{
			var foundProjects = new List<ProjectEntry>();
			foreach (var entry in ProjectEntries)
				if (entry.IsCSharpProject)
					foundProjects.Add(entry);

			return foundProjects;
		}

		public List<ProjectEntry> GetSolutionFolders()
		{
			var foundFolders = new List<ProjectEntry>();
			foreach (var entry in ProjectEntries)
				if (entry.IsSolutionFolder)
					foundFolders.Add(entry);

			return foundFolders;
		}

		public ProjectEntry GetCSharpProject(string projectNameOfSolution)
		{
			foreach (var project in GetCSharpProjects())
				if (project.Title == projectNameOfSolution)
					return project;

			throw new ProjectNotFoundInSolution(projectNameOfSolution);
		}

		public class ProjectNotFoundInSolution : Exception
		{
			public ProjectNotFoundInSolution(string projectName) : base(projectName) {}
		}
	}
}